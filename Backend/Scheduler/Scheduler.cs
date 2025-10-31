using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Scheduler
{
    public class Scheduler
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;
        private readonly IGlobals _globals;
        private readonly ILogger<Scheduler> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private readonly List<IRegistryProvider> _registryProviders = new List<IRegistryProvider>();
        private readonly List<Registry> _registries = new List<Registry>();

        public Scheduler(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider, IGlobals globals, ILogger<Scheduler> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
            _globals = globals;
            _logger = logger;
            _loggerFactory = loggerFactory;

            // Initialize Registries (consider lazy-init per job in future)
            var registriesFromDb = _context.Registries.AsNoTracking().ToList();
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
                        CreatedAt = DateTime.UtcNow,
                        RawMessage = e.ToString()
                    });
                    _logger.LogError(e, "Error initializing registry provider for {RegistryId}", registry.Id);
                    continue;
                }
                _registryProviders.Add(provider);
                _registries.Add(registry);
            }
        }

        // Backward-compatible constructor for existing call sites (e.g., controller)
        public Scheduler(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider, IGlobals globals)
            : this(context, utilities, providerDecider, globals, Microsoft.Extensions.Logging.Abstractions.NullLogger<Scheduler>.Instance, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance)
        { }

        public async Task RunOnceAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var jobs = await _context.Jobs.Where(b =>
                        (b.RunAfter <= now && !b.IsPermanent && !b.IsCompleted) ||
                        (b.RunAfter <= now && b.IsPermanent))
                    .Include(b => b.Cryptokey)
                    .Include(j => j.DnsServer)
                    .ToListAsync(ct);

            foreach (var job in jobs)
            {
                try
                {
                    _logger?.LogInformation("Dispatching job {JobId} {Task}", job.Id, job.Task);
                    switch (job.Task)
                    {
                        case JobName.CheckForDomainChanges:
                            var domainChanges = new DomainChanges(_context, job, _utilities, _providerDecider);
                            await domainChanges.ExecuteAsync(ct);
                            break;

                        case JobName.CheckDomain:
                            var checkDomains = new CheckDomain(_context, _utilities, _globals, job, _registryProviders, _registries, _loggerFactory?.CreateLogger<CheckDomain>());
                            await checkDomains.ExecuteAsync(ct);
                            break;

                        case JobName.SignDomain:
                            var signDomain = new SignDomain(_context, _utilities, _globals, job, _registryProviders, _registries, _loggerFactory?.CreateLogger<SignDomain>());
                            await signDomain.ExecuteAsync(ct);
                            break;

                        case JobName.UnSignDomain:
                            var unsign = new UnsignDomain(_context, _utilities, _globals, job, _registryProviders, _registries, _loggerFactory?.CreateLogger<UnsignDomain>());
                            await unsign.ExecuteAsync(ct);
                            break;

                        case JobName.KeyRolloverDomain:
                            var rollover = new KeyRolloverDomain(_context, _utilities, _globals, job, _registryProviders, _registries, _loggerFactory?.CreateLogger<KeyRolloverDomain>());
                            await rollover.ExecuteAsync(ct);
                            break;

                        default:
                            _context.Logs.Add(Logging.LogGeneral(LogType.Warning, $"Unsupported job type: {job.Task}", null));
                            break;
                    }
                }
                catch (Exception e)
                {
                    _context.Logs.Add(new Log
                    {
                        Message = $"Scheduler crashed while executing jobid: {job.Id} Exception: {e.Message}",
                        LogType = LogType.Error,
                        CreatedAt = DateTime.UtcNow,
                        RawMessage = e.ToString()
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
                    _context.Logs.Add(Logging.LogGeneral(LogType.Error, "Scheduler error while closing RegistryProviders: " + e.Message, e.ToString()));
                }
            }

            await _context.SaveChangesAsync(ct);
        }
    }

    //public static class Extensions
    //{
    //    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    //        => self.Select((item, index) => (item, index));
    //}
}
