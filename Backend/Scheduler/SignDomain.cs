using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Providers;
using Microsoft.Extensions.Logging;

namespace Backend.Scheduler
{
    public class SignDomain
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IGlobals _globals;
        private readonly Microsoft.Extensions.Logging.ILogger<SignDomain> _logger;

        private readonly Job _job;
        private Domain _domain;
        private IRegistryProvider _registryProvider;

        public SignDomain(ApplicationDbContext context, IUtilities utilities, IGlobals globals, Job job, List<IRegistryProvider> registryProviders, List<Registry> registries, Microsoft.Extensions.Logging.ILogger<SignDomain> logger = null)
        {
            _context = context;
            _utilities = utilities;
            _globals = globals;
            _logger = logger;

            _job = job;
            _globals.Registries = registries;
            _globals.RegistryProviders = registryProviders;
        }

        public void LogInfo(string message)
        {
            _logger?.LogInformation("{Message} for job {JobId}", message, _job.Id);
        }

        public void LogError(Exception e, string message)
        {
            _logger?.LogError(e, "{Message} for job {JobId}", message, _job.Id);
        }

        public async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken ct = default)
        {
            double failRerun;
            try
            {
                failRerun = Convert.ToDouble(_utilities.GetSetting("DomainSignFailRerun"));
            }
            catch (Exception e)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Config table has wrong value for 'DomainSignFailRerun' (not convertable to double), will retry in 1 day", e.ToString()));
                _job.RunAfter = DateTime.UtcNow.AddDays(1);
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            _domain = _context.Domains
                .Include(x => x.DnsServer)
                .Include(x => x.TopLevelDomain)
                .FirstOrDefault(x => x.Id == _job.DomainId);

            // Check if domain is found
            if (_domain == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "No domain found with ID " + _job.DomainId + " the job will now stop"));
                _job.IsCompleted = true;
                _job.IsSuccessful = false;
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            // Check if domain is Excluded from Signing
            if (_domain.ExcludeSigning)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Warning, "This domain is excluded from signing, the job will now stop"));
                _job.IsCompleted = true;
                _job.IsSuccessful = false;
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            // Check if the domain still has a Registry
            if (_domain.CustomRegistryId == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Warning, "This domain has no registry anymore (domain cancelled or transferred while signing?), the job will now stop"));
                _job.IsCompleted = true;
                _job.IsSuccessful = false;
                _job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
                return;
            }

            _registryProvider = _globals.GetRegistryProvider(_globals.Registries, _globals.RegistryProviders, _domain);
            if (_registryProvider == null)
            {
                _utilities.JobFail(_job, failRerun);
                return;
            }

            var init = InitJob();

            if (!init)
            {
                // Fails to initialize job, job fails here
                return;
            }

            switch (_job.Step)
            {
                case JobStep.SignDomainAtDnsServer:
                    SignDomainAtDnsServer();
                    break;
                case JobStep.UploadKeyToRegistry:
                    UploadKeyToRegistry();
                    break;
                case JobStep.SignDomainCompleted:
                    SignDomainCompleted();
                    break;
            }

            _job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }

        private bool InitJob()
        {
            var dnsProvider = _globals.GetDnsProvider(_domain);
            if (dnsProvider == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "DNS server could not be found"));
                return false;
            }

            var registryDnsSecKeys = _registryProvider.GetDomainInfo(_domain.Name).RegistryDnsSecs;

            ICollection<Providers.Dto.DnsZoneCryptokey> dnsZoneCryptokeys;
            try
            {
                dnsZoneCryptokeys = dnsProvider.GetZoneInfo(_domain.Name).DnsZoneCryptokeys;
            }
            catch (Exception)
            {
                var failRerun = Convert.ToDouble(_utilities.GetSetting("DomainSignFailRerun"));
                _job.IsCompleted = false;
                _job.IsSuccessful = false;
                _job.RunAfter = DateTime.UtcNow.AddMilliseconds(failRerun);
                _job.UpdatedAt = DateTime.UtcNow;
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Could not get the domain info from the DNS Server, check the DNS server logs"));
                return false;
            }


            // Initializing Job
            if (_job.Step == null)
            {
                // Is there another job running for this domain?
                var existingJob = ExistingJobs();
                if (existingJob)
                {
                    // Job is already running so job is cancelled
                    _job.IsSuccessful = false;
                    _job.IsCompleted = true;
                    _job.UpdatedAt = DateTime.UtcNow;
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Warning, "Another job was already running"));
                    _context.SaveChanges();
                    return false;
                }

                // Is there already a key?
                var dnssecStatus = _globals.GetDnssecStatus(_domain, _registryProvider);

                switch (dnssecStatus)
                {
                    case DnssecStatus.Broken:
                        _context.Logs.Add(Logging.LogJob(_job, LogType.Warning, "The domain was broken, the keys are deleted, we will now sign the domain"));
                        _globals.DeleteAllKeysFromDomain(_domain, _registryProvider, registryDnsSecKeys.ToList(),
                            dnsZoneCryptokeys.ToList());
                        break;
                    case DnssecStatus.Signed:
                        _domain.SignMatch = true;
                        _job.IsSuccessful = false;
                        _job.IsCompleted = true;
                        _job.UpdatedAt = DateTime.UtcNow;
                        _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "The domain was already signed succesfully"));
                        _context.SaveChanges();
                        return false;
                }

                // delete existing keys from DNS server (usually failed job keys left on the server)
                foreach (var key in dnsZoneCryptokeys)
                {
                    dnsProvider.UnSign(_domain.Name, key.Key);
                }

                _job.Step = JobStep.SignDomainAtDnsServer;
            }

            return true;
        }

        private bool ExistingJobs()
        {
            var existingJob = _context.Jobs
                .Any(x => x.DomainId == _job.DomainId && x.IsCompleted == false && x.Id != _job.Id);
            if (existingJob)
            {
                //_context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Another job is already running"));
                //_job.IsSuccessful = false;
                //_job.IsCompleted = true;
                //_context.SaveChanges();
                return true;
            }

            return false;
        }

        private void SignDomainAtDnsServer()
        {
            var cryptokey = _globals.SignDomainAtDnsServer(_job.Domain);
            _context.Add(cryptokey);
            _context.SaveChanges();

            _job.CryptokeyId = cryptokey.Id;
            _job.Step = JobStep.UploadKeyToRegistry;

            var defaultTtl = Convert.ToDouble(_utilities.GetSetting("DefaultTtl"));

            if (_domain.Ttl == 0)
            {
                _job.RunAfter = DateTime.UtcNow.AddSeconds(defaultTtl);
            }
            else
            {
                var ttl = defaultTtl;
                if (_domain.Ttl > defaultTtl)
                {
                    ttl = _domain.Ttl;
                }
                _job.RunAfter = DateTime.UtcNow.AddSeconds(ttl);
            }

            _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "SignDomainAtDnsServer step completed"));
        }

        private void UploadKeyToRegistry()
        {
            _globals.UploadKey(_job);
            _job.Step = JobStep.SignDomainCompleted;
            _job.RunAfter = DateTime.UtcNow.AddHours(2);

            _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "UploadKey step completed"));
        }

        private void SignDomainCompleted()
        {
            var isSigned = _globals.GetDnssecStatus(_domain, _registryProvider);
            var dbCryptokeys = _context.Cryptokeys.Where(x => x.Id == _job.CryptokeyId).ToList();
            switch (isSigned)
            {
                case DnssecStatus.Signed:
                    _domain.SignMatch = true;
                    _domain.SignedAt = DateTime.Now;
                    _job.IsSuccessful = true;
                    _job.CryptokeyId = null;
                    _job.Cryptokey = null;
                    _utilities.JobSuccess(_job);
                    _context.Cryptokeys.RemoveRange(dbCryptokeys);
                    break;
                case DnssecStatus.Broken:
                    _domain.SignMatch = false;
                    _domain.SignedAt = null;
                    _job.IsSuccessful = false;
                    _job.CryptokeyId = null;
                    _job.Cryptokey = null;
                    _utilities.JobSuccess(_job);
                    _context.Cryptokeys.RemoveRange(dbCryptokeys);
                    _globals.DeleteAllKeysFromDomain(_domain, _registryProvider);

                    _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "The domain was broken during the job, keys are deleted"));
                    break;
                case DnssecStatus.Unsigned:
                    var failRerun = Convert.ToDouble(_utilities.GetSetting("DomainSignFailRerun"));
                    _domain.SignMatch = false;
                    _job.IsCompleted = false;
                    _job.IsSuccessful = false;
                    _job.Step = JobStep.UploadKeyToRegistry;
                    _job.RunAfter = DateTime.Now.AddMilliseconds(failRerun);
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Job was unsuccessful, the domain could not be signed, trying key upload again later"));
                    break;
            }

            _context.SaveChanges();

        }
    }
}
