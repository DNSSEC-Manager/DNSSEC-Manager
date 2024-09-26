using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Nager.PublicSuffix;
using Providers;
using Providers.Dto;

namespace Backend.Business
{
    public interface IUtilities
    {
        string Protect(string unencryptedData);
        string Unprotect(string encryptedData);
        int GetTldId(string domain);
        void ProviderErrorToLog(ProviderResponse providerResponse, Job job);
        List<int> GetRegistryCheckOrder(List<Registry> registries, Domain domain);
        int GetRegistryIndexFromList(List<Registry> registries, Registry registry);
        SettingsTimeObject ConfigToTimeObject(Config config);
        double MinutesToDays(int minutes);
        double DaysToMinutes(int days);
        string HoursMinutesToMs(int hours, int minutes);
        string GetSetting(string key);
        int FindIfRegistriesHaveDomain(List<IRegistryProvider> registryProviders, Domain domain, List<int> indexes);
        bool RegistryKeysContainsDnsKey(List<RegistryDnsSec> registryDnsSecs, DnsZoneCryptokey dnsZoneCryptokey);
        bool DnsKeysContainsRegistryKey(RegistryDnsSec registryDnsSec, List<DnsZoneCryptokey> dnsZoneCryptokeys);
        void JobSuccess(Job job, double rerun = 0);
        void JobFail(Job job, double rerun);
        void JobFailNoRerun(Job job);
    }
    public class Utilities : IUtilities
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public Utilities(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _protector = dataProtectionProvider.CreateProtector("ProtectData"); // Purpose (not a secret)
        }

        public string Protect(string unencryptedData)
        {
            var encryptedData = "";
            try
            {
                encryptedData = _protector.Protect(unencryptedData);
            }
            catch (Exception ex)
            {
                _context.Logs.Add(Logging.LogGeneral(LogType.Error, "Unable to encrypt data: " + ex.Message));
                _context.SaveChanges();
            }
            return encryptedData;
        }

        public string Unprotect(string encryptedData)
        {
            var unencryptedData = "";
            try
            {
                unencryptedData = _protector.Unprotect(encryptedData);
            }
            catch (Exception ex)
            {
                _context.Logs.Add(Logging.LogGeneral(LogType.Error, "Unable to decrypt data: " + ex.Message));
                _context.SaveChanges();
            }
            return unencryptedData;
        }

        public int GetTldId(string domain)
        {
            var domainParser = new DomainParser(new WebTldRuleProvider());

            string tld;
            try
            {
                tld = domainParser.Parse(domain).TLD;
            }
            catch (Exception)
            {
                return -1;
            }

            var existingTld = _context.TopLevelDomains.FirstOrDefault(b => b.Tld == tld);
            if (existingTld != null)
            {
                return existingTld.Id;
            }

            //New TLD found
            var newTld = new TopLevelDomain
            {
                Tld = tld
            };

            _context.TopLevelDomains.Add(newTld);
            _context.Logs.Add(Logging.LogGeneral(LogType.Info, "New TLD found: " + newTld.Tld));
            _context.SaveChanges();
            return newTld.Id;
        }

        public void ProviderErrorToLog(ProviderResponse providerResponse, Job job)
        {
            if (!providerResponse.Success)
            {
                var newLog = new Log
                {
                    LogType = LogType.Warning,
                    Message = providerResponse.Error,
                    DomainId = job.DomainId,
                    CreatedAt = DateTime.Now,
                    JobId = job.Id,
                    RawMessage = providerResponse.RawMessage
                };
                _context.Logs.Add(newLog);
                _context.SaveChanges();
            }
        }

        public List<int> GetRegistryCheckOrder(List<Registry> registries, Domain domain)
        {
            var returnList = new List<int>();

            // If domain has no TLD then return empty list
            if (domain.TopLevelDomainId == null)
            {
                //foreach (var registry in registries)
                //{
                //    var index = GetRegistryIndexFromList(registries, topLevelDomainRegistry.Registry);
                //    returnList.Add(registry.index);
                //}
                return returnList;
            }

            // Get the TLD of the domain
            string tld = "";
            var topLevelDomain = _context.TopLevelDomains.FirstOrDefault(b => b.Id == domain.TopLevelDomainId);
            if (topLevelDomain != null)
            {
                tld = topLevelDomain.Tld;
            }

            // Check if we have a connected registry for our TLD (add on top of CheckOrder index list)
            var connectedTopLevelDomainRegistries = _context.TopLevelDomainRegistries
                .Include(b => b.TopLevelDomain)
                .Include(b => b.Registry)
                .Where(b => b.TopLevelDomain.Id == domain.TopLevelDomainId)
                .ToList();
            foreach (var topLevelDomainRegistry in connectedTopLevelDomainRegistries)
            {
                var index = GetRegistryIndexFromList(registries, topLevelDomainRegistry.Registry);
                returnList.Add(index);
            }

            // Loop through all registries and
            // Check if a registry has a connected TLD
            foreach (var registry in registries)
            {
                var topLevelDomainRegistries = _context.TopLevelDomainRegistries
                    .Include(b => b.Registry)
                    .Include(b => b.TopLevelDomain)
                    .Where(b => b.RegistryId == registry.Id)
                    .Select(b => b.TopLevelDomain.Tld)
                    .ToList();
                if (!topLevelDomainRegistries.Contains(tld) && topLevelDomainRegistries.Count > 0)
                {
                    continue;
                }
                var index = GetRegistryIndexFromList(registries, registry);
                if (!returnList.Contains(index))
                {
                    returnList.Add(index);
                }
            }

            return returnList;
        }

        // Returns the index of a registry in a list of registries
        public int GetRegistryIndexFromList(List<Registry> registries, Registry registry)
        {
            for (var i = 0; i < registries.Count; i++)
            {
                if (registries[i].Id == registry.Id)
                {
                    return i;
                }
            }
            return -1;
        }

        // Returns an object with Hours & minutes, calculated from milliseconds
        public SettingsTimeObject ConfigToTimeObject(Config config)
        {
            var settingsTimeObject = new SettingsTimeObject
            {
                Hours = Convert.ToInt32(Math.Round(Convert.ToDouble(config.Value) / 3600000))
            };

            long hoursInMs = settingsTimeObject.Hours * 3600000;
            var msLeft = Convert.ToInt64(config.Value) - hoursInMs;
            settingsTimeObject.Minutes = Convert.ToInt32(msLeft / 60000);

            return settingsTimeObject;
        }

        public double MinutesToDays(int minutes)
        {
            return minutes / 60 / 24;
        }

        public double DaysToMinutes(int days)
        {
            return days * 24 * 60;
        }

        // Returns the value in Milliseconds from given hours + minutes
        public string HoursMinutesToMs(int hours, int minutes)
        {
            return Convert.ToString(hours * 3600000 + minutes * 60000);
        }

        // Returns the value from the config table based on the given key
        public string GetSetting(string key)
        {
            var value = _context.Configs.Single(b => b.Key == key).Value;

            return value;
        }

        // returns the index of the registryprovider if one of them has a given domain
        public int FindIfRegistriesHaveDomain(List<IRegistryProvider> registryProviders, Domain domain, List<int> indexes)
        {
            var registryIndex = -1;

            foreach (var index in indexes)
            {
                var exists = registryProviders[index].DomainExists(domain.Name);
                if (exists)
                {
                    registryIndex = index;
                    break;
                }
            }

            return registryIndex;
        }

        // checks if a list of registry dnssec keys contains a dns zone key
        public bool RegistryKeysContainsDnsKey(List<RegistryDnsSec> registryDnsSecs, DnsZoneCryptokey dnsZoneCryptokey)
        {
            var returnBool = false;

            foreach (var registryDnsSec in registryDnsSecs)
            {
                if (dnsZoneCryptokey.Key == registryDnsSec.Key && dnsZoneCryptokey.Algo == registryDnsSec.Algo && dnsZoneCryptokey.Flag == registryDnsSec.Flag)
                {
                    returnBool = true;
                    break;
                }
            }
            return returnBool;
        }

        // Checks if a list of dns zone keys contains a registry key
        public bool DnsKeysContainsRegistryKey(RegistryDnsSec registryDnsSec, List<DnsZoneCryptokey> dnsZoneCryptokeys)
        {
            var returnBool = false;

            foreach (var dnsZoneCryptokey in dnsZoneCryptokeys)
            {
                if (dnsZoneCryptokey.Key == registryDnsSec.Key && dnsZoneCryptokey.Algo == registryDnsSec.Algo && dnsZoneCryptokey.Flag == registryDnsSec.Flag)
                {
                    returnBool = true;
                    break;
                }
            }
            return returnBool;
        }

        public void JobSuccess(Job job, double rerun = 0)
        {
            if (job.IsPermanent)
                job.RunAfter = DateTime.Now.AddMilliseconds(rerun);
            job.IsCompleted = true;
            job.UpdatedAt = DateTime.Now;
            _context.Add(Logging.LogJob(job, LogType.Success, "Job succesfully executed"));
            _context.SaveChanges();
        }

        public void JobFail(Job job, double rerun)
        {
            job.RunAfter = DateTime.Now.AddMilliseconds(rerun);
            job.UpdatedAt = DateTime.Now;
            _context.Add(Logging.LogJob(job, LogType.Warning, "Job failed, we are going to retry at {0}" + job.RunAfter.ToString()));
            _context.SaveChanges();
        }

        public void JobFailNoRerun(Job job)
        {
            job.IsCompleted = true;
            job.UpdatedAt = DateTime.Now;
            _context.Add(Logging.LogJob(job, LogType.Error, "Job failed, we are not going to retry"));
            _context.SaveChanges();
        }
    }
}
