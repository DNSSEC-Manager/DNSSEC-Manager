using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Providers;
using Providers.Dto;

namespace Backend.Scheduler
{
    public class KeyRolloverDomain
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IGlobals _globals;

        private readonly Job _job;        
        private readonly Domain _domain;
        private readonly IDnsProvider _dnsProvider;
        private readonly IRegistryProvider _registryProvider;
        private readonly Cryptokey _keyNotToDel;

        public KeyRolloverDomain(ApplicationDbContext context, IUtilities utilities, IGlobals globals, Job job, List<IRegistryProvider> registryProviders, List<Registry> registries)
        {
            _context = context;
            _job = job;
            _utilities = utilities;
            _globals = globals;
            _globals.Registries = registries;
            _globals.RegistryProviders = registryProviders;

            //_globals.Utilities = _utilities;
            //_globals = new Globals
            //{
            //    Utilities = new Utilities(_context),
            //    Context = _context,
            //    Registries = registries,
            //    RegistryProviders = registryProviders
            //};

            _keyNotToDel = _job.Cryptokey;

            _domain = _context.Domains.Include(b => b.DnsServer).Single(x => x.Id == _job.DomainId);

            _dnsProvider = _globals.GetDnsProvider(_domain);
            if (_dnsProvider == null)
            {
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "DNS server could not be found"));
                return;
            }

            _registryProvider = _globals.GetRegistryProvider(registries, registryProviders, _domain);
            if (_registryProvider == null)
            {
                _utilities.JobFailNoRerun(_job);
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Registry could not be found"));
                return;
            }

            // Initializing job
            var init = InitJob();

            if (!init)
            {
                // Fails to init job, job will stop here
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
                case JobStep.DeleteOldKeyFromRegistry:
                    DeleteOldKeyFromRegistry();
                    break;
                case JobStep.DeleteOldKeyFromDnsServer:
                    DeleteOldKeyFromDnsServer();
                    break;
            }

            _job.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

        }

        private void SignDomainAtDnsServer()
        {
            var cryptokey = _globals.SignDomainAtDnsServer(_job.Domain);
            _context.Add(cryptokey);

            _job.CryptokeyId = cryptokey.Id;

            _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "SingDomainAtDnsServer step completed"));
            _job.Step = JobStep.UploadKeyToRegistry;

            var defaultTtl = Convert.ToDouble(_utilities.GetSetting("DefaultTtl"));
            if (_domain.Ttl == 0)
            {
                _job.RunAfter = DateTime.Now.AddSeconds(defaultTtl);
            }
            else
            {
                var ttl = defaultTtl;
                if (_domain.Ttl > defaultTtl)
                {
                    ttl = _domain.Ttl;
                }
                _job.RunAfter = DateTime.Now.AddSeconds(ttl);
            }
        }

        private void UploadKeyToRegistry()
        {
            _globals.UploadKey(_job);
            _job.Step = JobStep.DeleteOldKeyFromRegistry;
            //TODO: Upload Complete is now 2 hours hard coded, make a checker to see if key is live
            _job.RunAfter = DateTime.Now.AddHours(2);
            _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "UploadKey step completed"));
        }

        private void DeleteOldKeyFromRegistry()
        {
            var registryDnsSecKeys = _registryProvider.GetDomainInfo(_domain.Name).RegistryDnsSecs;
            ProviderResponse response;
            var success = false;
            foreach (var key in registryDnsSecKeys)
            {
                if (key.Key != _keyNotToDel.Key || key.Flag != _keyNotToDel.Flag || key.Algo != _keyNotToDel.Algo)
                {
                    response = _registryProvider.DeleteKey(_domain.Name, key.Flag, key.Algo, key.Key);

                    if (!response.Success)
                    {
                        _context.Logs.Add(Logging.LogJob(_job, LogType.Error, response.Error + " (we will try again in 24 hours)"));
                        _job.RunAfter = DateTime.Now.AddHours(24); //try again in 24 hours
                    }
                    else
                    {
                        success = true;
                    }
                }
            }

            if (success)
            {
                _job.Step = JobStep.DeleteOldKeyFromDnsServer;
                //TODO: Upload Complete is now 2 hours hard coded, make a checker to see if key is live
                _job.RunAfter = DateTime.Now.AddHours(2);
                _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "DeleteOldKeyFromRegistry step completed"));
            }
        }

        private void DeleteOldKeyFromDnsServer()
        {
            ICollection<Providers.Dto.DnsZoneCryptokey> dnsZoneCryptokeys;
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
                if (key.Key != _keyNotToDel.Key || key.Flag != _keyNotToDel.Flag || key.Algo != _keyNotToDel.Algo)
                {
                    _dnsProvider.UnSign(_domain.Name, key.Key);
                }
            }

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
                    _context.Cryptokeys.RemoveRange(dbCryptokeys);
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "DeleteOldKeyFromDnsServer step completed"));
                    _utilities.JobSuccess(_job);
                    break;
                case DnssecStatus.Broken:
                    _domain.SignMatch = false;
                    _domain.SignedAt = null;
                    _job.IsCompleted = true;
                    _job.IsSuccessful = false;
                    _job.CryptokeyId = null;
                    _job.Cryptokey = null;
                    _context.Cryptokeys.RemoveRange(dbCryptokeys);
                    _globals.DeleteAllKeysFromDomain(_domain, _registryProvider);

                    _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "The domain was broken during the job, keys are deleted"));
                    break;
                case DnssecStatus.Unsigned:
                    _domain.SignMatch = false;
                    _domain.SignedAt = null;
                    _job.IsCompleted = true;
                    _job.IsSuccessful = false;
                    _job.CryptokeyId = null;
                    _job.Cryptokey = null;
                    _context.Cryptokeys.RemoveRange(dbCryptokeys);
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Job was unsuccessful, the domain was unsigned during the job"));
                    break;
            }
        }

        private bool InitJob()
        {
            var registryDnsSecKeys = _registryProvider.GetDomainInfo(_domain.Name).RegistryDnsSecs;
            var dnsZoneCryptokeys = _dnsProvider.GetZoneInfo(_domain.Name).DnsZoneCryptokeys;

            if (_job.Step == null)
            {
                // Is there another job running for this domain?
                var existingJob = _context.Jobs.Any(x =>
                    x.DomainId == _job.DomainId && x.IsCompleted == false && x.Id != _job.Id);
                if (existingJob)
                {
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "Another job is already running"));
                    _job.IsSuccessful = false;
                    _job.IsCompleted = true;
                }

                // Is there already a key?
                var dnssecStatus = _globals.GetDnssecStatus(_domain, _registryProvider);

                switch (dnssecStatus)
                {
                    case DnssecStatus.Broken:
                        _context.Logs.Add(Logging.LogJob(_job, LogType.Error,
                            "The domain was broken, keys are deleted, domain is not signed"));
                        _domain.SignMatch = false;
                        _job.IsSuccessful = false;
                        _job.IsCompleted = true;
                        _globals.DeleteAllKeysFromDomain(_domain, _registryProvider, registryDnsSecKeys.ToList(),
                            dnsZoneCryptokeys.ToList());
                        _context.SaveChanges();
                        return false;
                    case DnssecStatus.Unsigned:
                        _context.Logs.Add(Logging.LogJob(_job, LogType.Error,
                            "The domain was unsigned, cannot perform key rollover"));
                        _domain.SignMatch = false;
                        _job.IsSuccessful = false;
                        _job.IsCompleted = true;
                        _context.SaveChanges();
                        return false;
                }

                _job.Step = JobStep.SignDomainAtDnsServer;
            }
            return true;
        }
    }
}
