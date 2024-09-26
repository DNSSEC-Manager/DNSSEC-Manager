using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Providers;

namespace Backend.Scheduler
{
    public class CheckDomain
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IGlobals _globals;
        //private readonly bool _close;
        public Job _job;
        //public List<Registry> _initializedRegistries;

        //OLD VARIABLES:
        private readonly List<IRegistryProvider> _registryProviders;
        private readonly List<Registry> _registries;

        public CheckDomain(ApplicationDbContext context, IUtilities utilities, IGlobals globals, Job job, List<IRegistryProvider> registryProviders, List<Registry> registries)
        {
            _context = context;
            _utilities = utilities;
            _globals = globals;
            //_close = false;
            _job = job;
            //_initializedRegistries = initializedRegistries;

            _registryProviders = registryProviders;
            _registries = registries;

        }

        public void Start()
        {
            var amountOfDomainsToCheck = Convert.ToInt32(_utilities.GetSetting("CheckAlldomainsPerRun"));
            var msRunJobAgain = Convert.ToDouble(_utilities.GetSetting("CheckAllDomainsRunEvery"));
            var msCheckDomainAgainEvery = Convert.ToDouble(_utilities.GetSetting("CheckAllDomainsReRunAfter"));
            var datetimeValidHours = DateTime.Now.AddMilliseconds(msCheckDomainAgainEvery * -1);

            var domains = _context.Domains
                .Include(b => b.DnsServer)
                .Include(b => b.Registry)
                .Include(b => b.NameServerGroup)
                //.Where(b => (b.LastChecked < datetimeValidHours || b.LastChecked == null) && !b.RemovedFromDnsServer && !b.IsReservedByScheduler)
                .Where(b => (b.LastChecked < datetimeValidHours || b.LastChecked == null) && !b.RemovedFromDnsServer)
                .OrderBy(x => x.LastChecked)
                .Take(amountOfDomainsToCheck)
                .ToList();

            if (domains.Count == 0)
            {
                _job.RunAfter = DateTime.Now.AddMilliseconds(msRunJobAgain);
                _job.IsCompleted = true;
                _job.UpdatedAt = DateTime.Now;
                _context.Add(Logging.LogJob(_job, LogType.Info, "All domains are fresh."));
                _context.SaveChanges();
                return;
            }

            CheckListOfDomains(domains);

            _job.RunAfter = DateTime.Now.AddMilliseconds(msRunJobAgain);
            _job.IsCompleted = true;
            _job.UpdatedAt = DateTime.Now;
            string _checkedDomains = "";
            foreach (var domain in domains)
            {
                _checkedDomains += domain.Name + ", ";
            }
            _context.Add(Logging.LogJob(_job, LogType.Success, "Successfully checked " + domains.Count + " domains.", "Domains checked: " + _checkedDomains));
            _context.SaveChanges();
            //_utilities.JobSuccess(_job, msRetry);
        }

        public void CheckListOfDomains(List<Domain> domains)
        {
            //foreach (var domain in domains)
            //{
            //    domain.IsReservedByScheduler = true;
            //}
            //_context.SaveChanges();

            foreach (var domain in domains)
            {
                CheckTtl(domain);
                //CheckRegistry(domain);

                // Registry check
                var registryProvider = CheckRegistry(domain);
                if (registryProvider == null)
                {
                    domain.LastChecked = DateTime.Now;
                    //domain.IsReservedByScheduler = false;
                    _context.SaveChanges();
                    continue;
                }

                var hasMatchingNameservers = CheckNameservers(domain, registryProvider);

                if (hasMatchingNameservers)
                {
                    CheckDnssec(domain, registryProvider);
                }


                // If the domain has no registry, we cannot check for Nameservers and Dnssec
                //if (domain.Registry != null)
                //{
                //    CheckNameservers(domain, registryProvider);
                //    CheckDnssec(domain, registryProvider);
                //}


                //domain.IsReservedByScheduler = false;
                //if (domain.SignedAt == DateTime.MinValue || domain.SignedAt == null)
                //{
                //    domain.SignedAt = DateTime.Now;

                //}
                //_context.SaveChanges();

                // Automatic Key Rollover Check
                var automaticKeyRollover = Convert.ToBoolean(_utilities.GetSetting("AutomaticKeyRollover"));
                if (domain.SignMatch && automaticKeyRollover)
                {
                    var setting = Convert.ToInt32(_utilities.GetSetting("KeyRolloverTime"));
                    var dateTimeToCheck = DateTime.Now.AddMinutes(setting * -1);
                    if (domain.SignedAt <= dateTimeToCheck)
                    {
                        var jobsOnDomain = _context.Jobs.Where(b => b.IsCompleted == false && b.DomainId == domain.Id).ToList();
                        if (jobsOnDomain.Count == 0)
                        {
                            var newJob = new Job
                            {
                                Domain = domain,
                                DomainId = domain.Id,
                                Task = JobName.KeyRolloverDomain,
                                CreatedAt = DateTime.Now,
                                RunAfter = DateTime.Now
                            };
                            _context.Add(newJob);

                        }
                    }
                }
                domain.LastChecked = DateTime.Now;
                _context.SaveChanges();
            }

            //if (_close)
            //{
            //    foreach (var (registryProvider, index) in _registryProviders.WithIndex())
            //    {
            //        try
            //        {
            //            registryProvider.Close();
            //        }
            //        catch (Exception)
            //        {
            //            _context.Logs.Add(Logging.LogGeneral(LogType.Error,
            //                "Registry: " + _registries[index].Name + " Error: Connection can not be closed"));
            //        }
            //    }
            //}
        }

        private void CheckTtl(Domain domain)
        {
            var dnsProvider = _globals.GetDnsProvider(domain);
            if (dnsProvider == null)
            {
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Info, "DNS server could not be found"));
                return;
            }

            var oldTtl = domain.Ttl;
            var newTtl = dnsProvider.GetTtl(domain.Name);
            if (newTtl != oldTtl)
            {
                domain.Ttl = newTtl;
                _context.Logs.Add(Logging.LogDomain(domain, LogType.Info, "Time to live has changed to: " + newTtl));
                _context.SaveChanges();
            }

        }

        private IRegistryProvider CheckRegistry(Domain domain)
        {
            int registryIndex;
            var indexes = _utilities.GetRegistryCheckOrder(_registries, domain);
            // Registry check
            if (domain.Registry != null)
            {
                registryIndex = _utilities.GetRegistryIndexFromList(_registries, domain.Registry);
                if (registryIndex == -1 || !_registryProviders[registryIndex].DomainExists(domain.Name))
                {
                    registryIndex = _utilities.FindIfRegistriesHaveDomain(_registryProviders, domain, indexes);
                }
            }
            else
            {
                registryIndex = _utilities.FindIfRegistriesHaveDomain(_registryProviders, domain, indexes);
            }

            // No registry found
            if (registryIndex == -1)
            {
                domain.CustomRegistryId = null;
                domain.NameServerGroupId = null;
                domain.SignMatch = false;
                domain.SignedAt = null;
                _context.SaveChanges();
                return null;
            }

            // Registry found
            var registryProvider = _registryProviders[registryIndex];
            domain.Registry = _registries[registryIndex];
            domain.CustomRegistryId = _registries[registryIndex].Id;
            _context.SaveChanges();
            return registryProvider;
        }


        //private void CheckRegistry(Domain domain)
        //{
        //    // Check if domain still is registered at the current Registry
        //    if (domain.Registry != null)
        //    {
        //        //_context.Logs.Add(Logging.LogDomain(domain, LogType.Info, "Checking if domain is still at Registry: " + domain.Registry.Name));
        //        if (_initializedRegistries.Find(r => r.Id == domain.Registry.Id).RegistryProvider.DomainExists(domain.Name))
        //        {
        //            _context.Logs.Add(Logging.LogDomain(domain, LogType.Info, "Domain is still at the current Registry: " + domain.Registry.Name));
        //            return;
        //        }
        //        else
        //        {
        //            _context.Logs.Add(Logging.LogDomain(domain, LogType.Info, "Domain is not registered anymore at the current Registry:  + domain.Registry.ToString()"));
        //        }
        //    }

        //    // Search for domain
        //    // TODO: implement check order
        //    foreach (var registry in _initializedRegistries)
        //    {
        //        if (registry.RegistryProvider.DomainExists(domain.Name))
        //        {
        //            domain.Registry = registry;
        //            _context.Logs.Add(Logging.LogDomain(domain, LogType.Info, "Domain was found at Registry: " + domain.Registry.Name));
        //            return;
        //        }
        //    }

        //    // Domain was not found
        //    domain.Registry = null;
        //    domain.CustomRegistryId = null;
        //    domain.NameServerGroupId = null;
        //    domain.SignMatch = false;
        //    domain.SignedAt = null;
        //    _context.SaveChanges();
        //    return;


        //    //int registryIndex;
        //    //var indexes = _utilities.GetRegistryCheckOrder(_registries, domain);
        //    //// Registry check
        //    //if (domain.Registry != null)
        //    //{
        //    //    registryIndex = _utilities.GetRegistryIndexFromList(_registries, domain.Registry);
        //    //    if (registryIndex == -1 || !_registryProviders[registryIndex].DomainExists(domain.Name))
        //    //    {
        //    //        registryIndex = _utilities.FindIfRegistriesHaveDomain(_registryProviders, domain, indexes);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    registryIndex = _utilities.FindIfRegistriesHaveDomain(_registryProviders, domain, indexes);
        //    //}

        //    //// No registry found
        //    //if (registryIndex == -1)
        //    //{
        //    //    domain.CustomRegistryId = null;
        //    //    domain.NameServerGroupId = null;
        //    //    domain.SignMatch = false;
        //    //    domain.SignedAt = null;
        //    //    _context.SaveChanges();
        //    //    return null;
        //    //}

        //    //// Registry found
        //    //var registryProvider = _registryProviders[registryIndex];
        //    //domain.Registry = _registries[registryIndex];
        //    //domain.CustomRegistryId = _registries[registryIndex].Id;
        //    //_context.SaveChanges();
        //    //return registryProvider;
        //}

        private bool CheckNameservers(Domain domain, IRegistryProvider registryProvider)
        {
            // First determine if this domain currently has matching nameservers with it's DNS Server according to our database
            var hadMatchingNameServers = domain.NameServerGroup?.DnsServerId == domain.DnsServerId;

            // Get domain info from registry
            var registryDomainInfo = registryProvider.GetDomainInfo(domain.Name);

            if (!string.IsNullOrWhiteSpace(registryDomainInfo.Error))
            {
                var newLog = new Log
                {
                    LogType = LogType.Error,
                    Domain = domain,
                    CreatedAt = DateTime.Now,
                    Job = _job,
                    Message = "Error getting Domain info from registry: " + registryDomainInfo.Error
                };
                _context.Add(newLog);
                return false;
            }

            // Get all nameServerGroups
            var nameServerGroups = _context.NameServerGroups.Include(b => b.NameServers).ToList();

            // Check if we (still have) have matching nameservers
            var registryNameServers = registryDomainInfo.NameServers;

            //convert registryNameServers to lowercase
            registryNameServers = registryNameServers.ConvertAll(d => d.ToLower());

            var foundNameservers = false;
            var matchingNameservers = false;
            foreach (var nameServerGroup in nameServerGroups)
            {
                var dbNameServerList = nameServerGroup.NameServers.Select(nameServer => nameServer.Name).ToList();
                var areEquivalent = (dbNameServerList.Count == registryNameServers.Count) &&
                                    !dbNameServerList.Except(registryNameServers).Any();
                // Nameserver found
                if (areEquivalent)
                {
                    domain.NameServerGroupId = nameServerGroup.Id;
                    _context.Update(domain);
                    _context.SaveChanges();
                    foundNameservers = true;

                    // Check if Nameservers are matching our DNS Server
                    if (nameServerGroup.DnsServerId == domain.DnsServerId)
                    {
                        matchingNameservers = true;
                    }

                    break;
                }
            }

            // New Nameservers found, add to database
            if (!foundNameservers)
            {
                // Generate unique name for the new nameserver group
                var uniqueName = "NewNameserverGroup" + DateTime.Now.Ticks.ToString();
                var newNameserverGroup = new NameServerGroup { Name = uniqueName };
                _context.Add(newNameserverGroup);
                _context.SaveChanges();

                // Add new nameservers to the new nameserver group
                foreach (var newNameserver in registryNameServers)
                {
                    var addNewNameserverToGroup = new NameServer { Name = newNameserver, NameServerGroupId = newNameserverGroup.Id };
                    _context.Add(addNewNameserverToGroup);
                }

                // Connect domainnamen with these nameservers
                domain.NameServerGroupId = newNameserverGroup.Id;
                _context.Update(domain);
                _context.SaveChanges();
            }


            // No matching nameserver found 
            if (!matchingNameservers)
            {
                // if the domain used to have nameservers it creates an unsign job
                if (hadMatchingNameServers)
                {
                    var newJob = new Job
                    {
                        Domain = domain,
                        CreatedAt = DateTime.Now,
                        RunAfter = DateTime.Now,
                        Task = JobName.UnSignDomain
                    };
                    var newLog = new Log
                    {
                        Domain = domain,
                        CreatedAt = DateTime.Now,
                        Job = _job,
                        Message = "The domain no longer has matching nameservers, the domain will be unsigned"
                    };
                    _context.Add(newJob);
                    _context.Add(newLog);
                }
                //domain.NameServerGroupId = null;
                //domain.SignedAt = null;
                _context.SaveChanges();
                return false;
            }

            //_context.SaveChanges();
            return matchingNameservers;
        }

        private void CheckDnssec(Domain domain, IRegistryProvider registryProvider)
        {
            //var dnssecStatus = _globals.GetDnssecStatus(domain, _initializedRegistries.Find(r => r.Id == domain.Registry.Id)?.RegistryProvider);
            var dnssecStatus = _globals.GetDnssecStatus(domain, registryProvider);
            var automaticSign = Convert.ToBoolean(_utilities.GetSetting("AutomaticSign"));
            var automaticFix = Convert.ToBoolean(_utilities.GetSetting("AutomaticFix"));
            var jobsOnDomain = _context.Jobs.Where(b => b.IsCompleted == false && b.DomainId == domain.Id).ToList();
            switch (dnssecStatus)
            {
                case DnssecStatus.Signed:
                    domain.SignMatch = true;
                    break;
                case DnssecStatus.Unsigned:
                    domain.SignMatch = false;
                    if (automaticSign && jobsOnDomain.Count == 0 && !domain.ExcludeSigning)
                    {
                        _context.Jobs.Add(new Job
                        {
                            Domain = domain,
                            DomainId = domain.Id,
                            CreatedAt = DateTime.Now,
                            RunAfter = DateTime.Now,
                            Task = JobName.SignDomain
                        });
                        _context.SaveChanges();
                    }
                    break;
                case DnssecStatus.Broken:
                    domain.SignMatch = false;
                    if (automaticFix && jobsOnDomain.Count == 0 && !domain.ExcludeSigning)
                    {
                        _context.Jobs.Add(new Job
                        {
                            Domain = domain,
                            DomainId = domain.Id,
                            CreatedAt = DateTime.Now,
                            RunAfter = DateTime.Now,
                            Task = JobName.SignDomain
                        });
                    }
                    break;
            }
            _context.SaveChanges();
        }
    }
}
