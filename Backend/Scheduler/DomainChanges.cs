using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Nager.PublicSuffix;
using Providers;

namespace Backend.Scheduler
{
    public class DomainChanges
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;

        private readonly Job _job;
        private readonly IDnsProvider _dnsProvider;
        private readonly DnsServer _dnsServer;

        public DomainChanges(ApplicationDbContext context, Job job, IUtilities utilities, IProviderDecider providerDecider)
        {
            _context = context;
            _job = job;
            _utilities = utilities;
            _providerDecider = providerDecider;

            _dnsServer = _job.DnsServer;
            try
            {
                _dnsProvider = _providerDecider.DnsProvider(_dnsServer);
            }
            catch (Exception e)
            {
                // DNS provider not found
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, e.Message));
                return;
            }

            var zones = GetZones();
            if (zones == null)
            {
                // Could not get the zones, job fails here
                return;
            }

            var activeDomainsFromDb = _context.Domains
                .Where(d => d.DnsServerId == _dnsServer.Id && !d.RemovedFromDnsServer)
                .Select(d => d.Name)
                .ToList();
            var newDomains = zones.Except(activeDomainsFromDb)
                .ToList();
            var removedDomains = activeDomainsFromDb.Except(zones)
                .ToList();

            AddNewDomains(newDomains);
            RemovedDomains(removedDomains);

            _job.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            _utilities.JobSuccess(_job, Convert.ToDouble(_utilities.GetSetting("DomainChangesRunEvery")));

        }

        private List<string> GetZones()
        {
            try
            {
                // Get all zones from the DNS server
                var zones = _dnsProvider.GetZones();
                return zones;
            }
            catch (Exception e)
            {
                //wat doet die 5? retry job na 5 seconden
                _utilities.JobFail(_job, 5);
                _context.Logs.Add(Logging.LogJob(_job, LogType.Error, "The job failed, error:" + e.Message));
                return null;
            }
        }

        private void AddNewDomains(List<string> newDomains)
        {
            var addList = new List<Domain>(100000);
            foreach (var newDomain in newDomains)
            {
                var newDomainLowercase = newDomain.ToLower();

                var existingDomain = _context.Domains
                    .FirstOrDefault(x => x.Name == newDomainLowercase && x.DnsServerId == _dnsServer.Id);
                if (existingDomain != null)
                {
                    existingDomain.RemovedFromDnsServer = false;
                    existingDomain.LastChecked = DateTime.Now;

                    _context.Logs.Add(Logging.LogJob(_job, LogType.Info,
                        "Reactivating existing domain: " + existingDomain.Name));
                }
                else
                {
                    var tildId = _utilities.GetTldId(newDomainLowercase);
                    if (tildId != -1)
                    {
                        addList.Add(new Domain
                        {
                            Name = newDomainLowercase,
                            DnsServerId = _dnsServer.Id,
                            CreatedAt = DateTime.Now,
                            TopLevelDomainId = tildId,
                            Ttl = _dnsProvider.GetTtl(newDomainLowercase),
                            SignedAt = DateTime.Now
                        });

                        _context.Logs.Add(Logging.LogJob(_job, LogType.Info, "New domain added: " + newDomainLowercase));
                    }
                }
            }

            _context.Domains.AddRange(addList);
        }

        private void RemovedDomains(List<string> removedDomains)
        {
            foreach (var removedDomain in removedDomains)
            {
                var existingDomain =
                    _context.Domains.FirstOrDefault(x => x.Name == removedDomain && x.DnsServerId == _dnsServer.Id);
                if (existingDomain != null)
                {
                    existingDomain.RemovedFromDnsServer = true;
                    existingDomain.LastChecked = DateTime.Now;
                    existingDomain.CustomRegistryId = null;
                    existingDomain.NameServerGroupId = null;
                    existingDomain.SignMatch = false;
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Info,
                        "Domain removed from DNS server " + existingDomain.Name));
                }
                else
                {
                    _context.Logs.Add(Logging.LogJob(_job, LogType.Error,
                        "Could not find " + removedDomain + " in the database"));
                }
            }
        }
    }
}
