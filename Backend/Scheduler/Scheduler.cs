using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Scheduler
{
    public class Scheduler
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;
        private readonly IGlobals _globals;

        private readonly List<Job> _jobs;
        //private readonly List<Registry> _initializedRegistries = new List<Registry>();

        //OLD VARIABLES:
        //private readonly List<IRegistryProvider> _registryProviders;
        //private readonly List<Registry> _registries;
        private readonly List<IRegistryProvider> _registryProviders = new List<IRegistryProvider>();
        private readonly List<Registry> _registries = new List<Registry>();

        public Scheduler(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider, IGlobals globals)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
            _globals = globals;

            // Get all jobs to run
            _jobs = _context.Jobs.Where(b =>
                    (b.RunAfter <= DateTime.Now && !b.IsPermanent && !b.IsCompleted) ||
                    (b.RunAfter <= DateTime.Now && b.IsPermanent))
                    .Include(b => b.Cryptokey)
                    .Include(j => j.DnsServer)
                    .ToList();

            // Initialize Registries
            var registriesFromDb = _context.Registries.ToList();
            //foreach (var registry in registriesFromDb)
            //{
            //    registry.RegistryProvider = _providerDecider.InitializeRegistryProvider(registry);
            //    _initializedRegistries.Add(registry);
            //}

            // OLD METHOD:
            //_registryProviders = new List<IRegistryProvider>();
            //_registries = new List<Registry>();
            //var registriesFromDb = _context.Registries.ToList();
            foreach (var registry in registriesFromDb)
            {
                IRegistryProvider provider;
                try
                {
                    provider = _providerDecider.InitializeRegistryProvider(registry);
                }
                catch (Exception e)
                {
                    _context.Logs.Add(new Log
                    {
                        Message = "Scheduler error while initializing RegistryProviders: " + e.Message,
                        LogType = LogType.Error,
                        RegistryId = registry.Id,
                        CreatedAt = DateTime.Now,
                        RawMessage = e.InnerException.ToString()
                    });

                    _context.SaveChanges();
                    continue;
                }
                _registryProviders.Add(provider);
                _registries.Add(registry);
            }

        }

        public void Run()
        {
            foreach (var job in _jobs)
            {
                try
                {
                switch (job.Task)
                {
                    case JobName.CheckForDomainChanges:
                        new DomainChanges(_context, job, _utilities, _providerDecider);
                        break;

                    case JobName.CheckDomain:
                        var checkDomains = new CheckDomain(_context, _utilities, _globals, job, _registryProviders, _registries);
                        checkDomains.Start();
                        break;

                    case JobName.SignDomain:
                        new SignDomain(_context, _utilities, _globals, job, _registryProviders, _registries);
                        break;

                    case JobName.UnSignDomain:
                        new UnsignDomain(_context, _utilities, _globals, job, _registryProviders, _registries);
                        break;

                    case JobName.KeyRolloverDomain:
                        new KeyRolloverDomain(_context, _utilities, _globals, job, _registryProviders, _registries);
                        break;
                }
                }
                catch (Exception e)
                {
                    if (e != null)
                    {
                        _context.Logs.Add(new Log
                        {
                            Message = "Scheduler crashed while executing jobid: " + job.Id + " Exception: " + e.Message,
                            LogType = LogType.Error,
                            CreatedAt = DateTime.Now,
                            RawMessage = e.InnerException.ToString()
                        });
                    }
                    _context.Logs.Add(new Log
                    {
                        Message = "Scheduler crashed while executing jobid: " + job.Id,
                        LogType = LogType.Error,
                        CreatedAt = DateTime.Now,
                    });
                }
            }

            foreach (var registry in _registryProviders)
            {
                try
                {
                    registry.Close();
                }
                catch (Exception e)
                {
                    _context.Logs.Add(Logging.LogGeneral(LogType.Error, "Scheduler error while closing RegistryProviders: " + e.Message, e.InnerException.ToString()));
                }
            }

            //foreach (var (registryProvider, index) in _registryProviders.WithIndex())
            //{
            //    try
            //    {
            //        registryProvider.Close();
            //    }
            //    catch (Exception)
            //    {
            //        _context.Logs.Add(Logging.LogGeneral(LogType.Error,
            //            "Registry: " + _registries[index].Name + " Error: Connection can not be closed"));
            //    }
            //}

            _context.SaveChanges();
        }
    }

    //public static class Extensions
    //{
    //    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    //        => self.Select((item, index) => (item, index));
    //}
}
