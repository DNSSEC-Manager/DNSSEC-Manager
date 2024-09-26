using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Providers;
using Providers.Dto;

namespace Backend.Scheduler
{
    public interface IGlobals
    {
        List<Registry> Registries { get; set; }
        List<IRegistryProvider> RegistryProviders { get; set; }
        IDnsProvider GetDnsProvider(Domain domain);
        IRegistryProvider GetRegistryProvider(List<Registry> registries, List<IRegistryProvider> registryProviders, Domain domain);
        DnssecStatus GetDnssecStatus(Domain domain, IRegistryProvider registryProvider);
        Cryptokey SignDomainAtDnsServer(Domain domain);
        void UploadKey(Job job);
        void DeleteAllKeysFromDomain(Domain domain, IRegistryProvider registryProvider = null,
            List<RegistryDnsSec> registryDnsSecs = null, List<DnsZoneCryptokey> dnsZoneCryptokeys = null);

    }
    public class Globals : IGlobals
    {
        public List<IRegistryProvider> RegistryProviders { get; set; }
        public List<Registry> Registries { get; set; }

        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;

        public Globals(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
        }


        public IDnsProvider GetDnsProvider(Domain domain)
        {
            try
            {
                return _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IRegistryProvider GetRegistryProvider(List<Registry> registries, List<IRegistryProvider> registryProviders, Domain domain)
        {
            var registryIndex = _utilities.GetRegistryIndexFromList(registries, domain.Registry);
            if (registryIndex == -1)
            {
                return null;
            }

            return registryProviders[registryIndex];
        }

        public DnssecStatus GetDnssecStatus(Domain domain, IRegistryProvider registryProvider)
        {
            var dnsProvider = _providerDecider.DnsProvider(domain.DnsServer);
            var registryDnsSecKeys = registryProvider.GetDomainInfo(domain.Name).RegistryDnsSecs;

            ICollection<DnsZoneCryptokey> dnsZoneCryptokeys;
            try
            {
                dnsZoneCryptokeys = dnsProvider.GetZoneInfo(domain.Name).DnsZoneCryptokeys;
            }
            catch (Exception e)
            {
                var log = new Log
                {
                    Domain = domain,
                    DnsServer = domain.DnsServer,
                    CreatedAt = DateTime.Now,
                    Message = "Could not get the domain info from the DNS Server",
                    LogType = LogType.Error,
                    RawMessage = e.Message
                };

                _context.Logs.Add(log);
                _context.SaveChanges();
                return DnssecStatus.Error;
            }



            if (registryDnsSecKeys?.Count > 0)
            {
                foreach (var registryDnsSecKey in registryDnsSecKeys)
                {
                    var thisKeyInDns =
                        _utilities.DnsKeysContainsRegistryKey(registryDnsSecKey, dnsZoneCryptokeys.ToList());
                    if (!thisKeyInDns)
                    {
                        return DnssecStatus.Broken;
                    }

                    return DnssecStatus.Signed;
                }
            }
            return DnssecStatus.Unsigned;
        }

        private Algorithm GetAlgoFromDomain(Domain domain)
        {
            var tld = _context.TopLevelDomains.FirstOrDefault(b => b.Id == domain.TopLevelDomainId);
            Algorithm algo;
            if (tld == null)
            {
                algo = _context.Algorithms.FirstOrDefault(b => b.IsDefault);
            }
            else
            {
                algo = _context.Algorithms.FirstOrDefault(b => b.Id == tld.OverrideAlgorithmId);
                if (algo == null)
                {
                    algo = _context.Algorithms.FirstOrDefault(b => b.IsDefault);
                }
            }

            if (algo == null)
            {
                // Should never happen, this would mean there is not even a default algo
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Error, "No suitable algorithm was found"));
                return null;
            }
            return algo;
        }

        public Cryptokey SignDomainAtDnsServer(Domain domain)
        {
            var dnsProvider = GetDnsProvider(domain);
            if (dnsProvider == null)
            {
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Error, "DNS provider could not be found"));
                return null;
            }

            var algo = GetAlgoFromDomain(domain);
            if (algo == null)
            {
                return null;
            }

            var signData = dnsProvider.Sign(domain.Name, algo.Name, algo.Bits);

            if (signData.Algo == null || signData.Flag == null || signData.Key == null)
            {
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Error,
                    "Could not sign the domain at the dns server"));
                return null;
            }

            return new Cryptokey
            {
                DomainId = domain.Id,
                Flag = signData.Flag,
                Algo = signData.Algo,
                Key = signData.Key,
                KeyTag = signData.KeyTag
            };
        }

        public void UploadKey(Job job)
        {
            var failRerun = Convert.ToDouble(_utilities.GetSetting("DomainSignFailRerun"));
            var domain = _context.Domains.Single(x => x.Id == job.DomainId);

            var registryProvider = GetRegistryProvider(Registries, RegistryProviders, domain);
            if (registryProvider == null)
            {
                _utilities.JobFail(job, failRerun);
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Error, "Registry provider could not be found"));
                return;
            }

            var cryptokey = _context.Cryptokeys.FirstOrDefault(c => c.Id == job.CryptokeyId);
            if (cryptokey != null)
            {
                // TODO: We could check if the DNSSEC key still exists at the DNS Server...
                // Or we just upload and let the checker find out if something is not in sync later...
                var response = registryProvider.Sign(domain.Name, cryptokey.Flag, cryptokey.Algo, cryptokey.Key, cryptokey.KeyTag);
                _utilities.ProviderErrorToLog(response, job);
                //Context.Logs.Add(Logging.LogJob(job, LogType.Success, "DNSSEC Key succesfully upload"));
            }
            else
            {
                _context.Logs.Add(Logging.LogJob(job, LogType.Error, "Could not find the key to upload"));
            }

            _context.SaveChanges();
        }

        public void DeleteAllKeysFromDomain(Domain domain, IRegistryProvider registryProvider = null,
            List<RegistryDnsSec> registryDnsSecs = null, List<DnsZoneCryptokey> dnsZoneCryptokeys = null)
        {
            if (registryProvider == null)
            {
                registryProvider = GetRegistryProvider(Registries, RegistryProviders, domain);
                if (registryProvider == null)
                {
                    _context.Logs.Add(Logging.LogDomain(domain, LogType.Error, "Registry provider could not be found"));
                    return;
                }
            }

            var dnsProvider = GetDnsProvider(domain);
            if (dnsProvider == null)
            {
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Error, "DNS provider could not be found"));
                return;
            }

            if (registryDnsSecs == null)
            {
                registryDnsSecs = registryProvider.GetDomainInfo(domain.Name).RegistryDnsSecs.ToList();
            }

            foreach (var registryDnsSec in registryDnsSecs)
            {
                var resp = registryProvider.DeleteKey(domain.Name, registryDnsSec.Flag, registryDnsSec.Algo,
                    registryDnsSec.Key);

                if (!resp.Success)
                {
                    _context.Logs.Add(Logging.LogDomain(domain, LogType.Error, "Failed to delete the dnssec keys"));
                    return;
                }
            }

            if (dnsZoneCryptokeys == null)
            {
                dnsZoneCryptokeys = dnsProvider.GetZoneInfo(domain.Name).DnsZoneCryptokeys.ToList();
            }

            foreach (var dnsZoneCryptokey in dnsZoneCryptokeys)
            {
                var resp = dnsProvider.UnSign(domain.Name, dnsZoneCryptokey.Key);
                if (!resp.Success)
                {
                    _context.Logs.Add(Logging.LogDomain(domain, LogType.Error,
                        "Failed to delete the dnssec key from the DNS server"));
                    return;
                }
            }


            _context.SaveChanges();
        }
    }
}
