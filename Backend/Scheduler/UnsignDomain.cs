using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Backend.Models.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Providers;
using Providers.Dto;

namespace Backend.Scheduler
{
    public class UnsignDomain
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IGlobals _globals;
        private readonly Microsoft.Extensions.Logging.ILogger<UnsignDomain> _logger;

        private readonly Job _job;
        private IDnsProvider _dnsProvider;
        private Domain _domain;
        private IRegistryProvider _registryProvider;

        public UnsignDomain(ApplicationDbContext context, IUtilities utilities, IGlobals globals, Job job, List<IRegistryProvider> registryProviders, List<Registry> registries, Microsoft.Extensions.Logging.ILogger<UnsignDomain> logger = null)
        {
            _context = context;
            _utilities = utilities;
            _globals = globals;
            _logger = logger;

            _job = job;
            _globals.Registries = registries;
            _globals.RegistryProviders = registryProviders;
        }

        public async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken ct = default)
        {
            _domain = _context.Domains.Include(b => b.DnsServer).SingleOrDefault(x => x.Id == _job.DomainId);
            if (_domain == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "No domain found with ID " + _job.DomainId));
                _job.IsCompleted = true;
                _job.IsSuccessful = false;
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            _dnsProvider = _globals.GetDnsProvider(_domain);
            if (_dnsProvider == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "DNS server could not be found"));
                _job.IsCompleted = true;
                _job.IsSuccessful = false;
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            _registryProvider = _globals.GetRegistryProvider(_globals.Registries, _globals.RegistryProviders, _domain);
            if (_registryProvider == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Registry could not be found"));
                _job.IsCompleted = true;
                _job.IsSuccessful = false;
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            // Initializing Job
            var initJob = InitJob();

            if (!initJob)
            {
                await _context.SaveChangesAsync(ct);
                return;
            }

            // Unsign steps
            switch (_job.Step)
            {
                case JobStep.DeleteAllKeysFromRegistry:
                    DeleteAllKeysFromRegistry();
                    break;
                case JobStep.DeleteAllKeysFromDnsServer:
                    DeleteAllKeysFromDnsServer();
                    break;
            }

            _job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }

        private bool InitJob()
        {
            var registryDnsSecKeys = _registryProvider.GetDomainInfo(_domain.ToDomainData()).RegistryDnsSecs.ToList();
            if (_job.Step == null)
            {
                // Is there another _job running for this domain?
                var existingJob = _context.Jobs.Any(x =>
                    x.DomainId == _job.DomainId && x.IsCompleted == false && x.Id != _job.Id);
                if (existingJob)
                {
                    _job.IsSuccessful = false;
                    _job.IsCompleted = true;
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Another job is already running"));
                    return false;
                }

                // Do we have a signed domain (are there DNSSEC keys at registry)?
                if (registryDnsSecKeys.Count == 0)
                {
                    _domain.SignMatch = false;
                    _domain.SignedAt = null;
                    _job.IsSuccessful = false;
                    _job.IsCompleted = true;
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "There are no DNSSEC keys at registry"));
                    return false;
                }
                _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "Domain is ready for unsigning"));
                _job.Step = JobStep.DeleteAllKeysFromRegistry;

                _context.SaveChanges();
            }
            return true;
        }

        private void DeleteAllKeysFromRegistry()
        {
            var registryDnsSecKeys = _registryProvider.GetDomainInfo(_domain.ToDomainData()).RegistryDnsSecs.ToList();
            ProviderResponse response;
            var success = false;

            foreach (var key in registryDnsSecKeys)
            {
                response = _registryProvider.DeleteKey(_domain.ToDomainData(), key.Flag, key.Algo, key.Key);
                if (!response.Success)
                {
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error, response.Error + " (we will try again in 24 hours)"));
                    _job.RunAfter = DateTime.UtcNow.AddHours(24); //try again in 24 hours
                }
                else
                {
                    success = true;
                }
            }

            if (success)
            {
                _domain.SignMatch = false;
                _domain.SignedAt = null;
                _job.Step = JobStep.DeleteAllKeysFromDnsServer;
                //TODO: TTL is now 2 hours hard coded, we could check if the keys are gone via DNS
                _job.RunAfter = DateTime.UtcNow.AddHours(2);
                _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "DeleteAllKeysFromRegistry step completed"));
            }

        }

        private void DeleteAllKeysFromDnsServer()
        {
            ICollection<DnsZoneCryptokey> dnsZoneCryptokeys;

            try
            {
                dnsZoneCryptokeys = _dnsProvider.GetZoneInfo(_domain.Name).DnsZoneCryptokeys;
            }
            catch (Exception)
            {
                var failRerun = Convert.ToDouble(_utilities.GetSetting("DomainSignFailRerun"));
                _job.IsCompleted = false;
                _job.IsSuccessful = false;
                _job.RunAfter = DateTime.Now.AddMilliseconds(failRerun);
                _job.UpdatedAt = DateTime.Now;
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Could not get the domain info from the DNS Server, check the DNS server logs"));
                return;
            }

            foreach (var key in dnsZoneCryptokeys)
            {
                _dnsProvider.UnSign(_domain.Name, key.Key);
            }
            _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "DeleteAllKeysFromDnsServer step completed"));
            _job.IsSuccessful = true;
            _utilities.JobSuccess(_job);
        }
    }
}
