using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Scheduler;
using Backend.ViewModels;
using Providers;
using Providers.Dto;
using Microsoft.AspNetCore.DataProtection;

namespace Backend.Controllers
{
    public class DomainsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;
        private readonly IGlobals _globals;

        public DomainsController(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider, IGlobals globals)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
            _globals = globals;
        }

        // GET: Domains
        public async Task<IActionResult> Index(int? pageNumber, string sort, string search, int? registryId, int? dnsId, int? nameserversId, int? removed, int? signed, int? pageSize)
        {
            var domains = await _context.Domains.Include(d => d.DnsServer).Include(d => d.NameServerGroup).Include(d => d.Registry).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                domains = domains.Where(b => b.Name.Contains(search)).ToList();
            }

            switch (registryId)
            {
                case -1:
                    domains = domains.Where(b => b.CustomRegistryId == null).ToList();
                    break;
                case 0:
                    domains = domains.Where(b => b.CustomRegistryId != null).ToList();
                    break;
                default:
                    if (registryId != null)
                    {
                        domains = domains.Where(b => b.CustomRegistryId == registryId).ToList();
                    }
                    break;
            }

            if (dnsId != null)
            {
                domains = domains.Where(b => b.DnsServerId == dnsId).ToList();
            }

            switch (nameserversId)
            {
                case -1:
                    domains = domains.Where(b => b.NameServerGroupId == null).ToList();
                    break;
                case 0:
                    domains = domains.Where(b => b.NameServerGroupId != null).ToList();
                    break;
                default:
                    if (nameserversId != null)
                    {
                        domains = domains.Where(b => b.NameServerGroupId == nameserversId).ToList();
                    }
                    break;
            }

            if (removed != null)
            {
                var boolValue = removed != 0;
                domains = domains.Where(b => b.RemovedFromDnsServer == boolValue).ToList();
            }

            if (signed != null)
            {
                var boolValue = signed != 0;
                domains = domains.Where(b => b.SignMatch == boolValue).ToList();
            }

            switch (sort)
            {
                case "name":
                    domains = domains.OrderBy(b => b.Name).ToList();
                    break;
                case "name_desc":
                    domains = domains.OrderByDescending(b => b.Name).ToList();
                    break;
                case "lastChecked":
                    domains = domains.OrderBy(b => b.LastChecked).ToList();
                    break;
                case "lastChecked_desc":
                    domains = domains.OrderByDescending(b => b.LastChecked).ToList();
                    break;
                case "ttl":
                    domains = domains.OrderBy(b => b.Ttl).ToList();
                    break;
                case "ttl_desc":
                    domains = domains.OrderByDescending(b => b.Ttl).ToList();
                    break;
                default:
                    sort = "name";
                    domains = domains.OrderBy(b => b.Name).ToList();
                    break;
            }
            int actualPageSize;

            if (pageSize == null)
            {
                actualPageSize = 10;
            }
            else
            {
                actualPageSize = (int)pageSize;
            }
            var page = pageNumber ?? 1;
            var skipNum = (page - 1) * actualPageSize;

            var totalDomains = domains.Count();
            var domainsList = domains.Skip(skipNum).Take(actualPageSize).ToList();
            var hasPreviousPage = false;

            var hasNextPage = false;
            var totalPages = (int)Math.Ceiling(totalDomains / (double)actualPageSize);
            if (pageNumber > 1)
            {
                hasPreviousPage = true;
            }

            if (page < totalPages)
            {
                hasNextPage = true;
            }

            var vm = new DomainsIndexViewModel
            {
                AutomaticSigningEnabled = Convert.ToBoolean(_context.Configs.Single(b => b.Key == "AutomaticSign").Value),
                Domains = domainsList,
                SelectionDomainsCount = totalDomains,
                Search = search,
                RegistryId = registryId,
                DnsId = dnsId,
                NameserversId = nameserversId,
                Sort = sort,
                Removed = removed,
                Signed = signed,
                PageNumber = page,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage,
                DnsServers = _context.DnsServers.ToList(),
                Registries = _context.Registries.ToList(),
                NameServerGroups = _context.NameServerGroups.ToList(),
                PageSize = actualPageSize
            };

            return View(vm);
        }

        //CleanupAllDomains
        [HttpPost]
        public string CleanupAllDomains()
        {
            var domains = _context.Domains.Where(b => b.RemovedFromDnsServer).ToList();
            foreach (var domain in domains)
            {
                //remove
                RemoveDomainFromDatabase(domain);
            }
            _context.SaveChanges();
            return "success";
        }

        [HttpPost]
        public string SignAllDomains()
        {
            var domains = _context.Domains.Where(b => !b.SignMatch && b.CustomRegistryId != null && b.NameServerGroupId != null).ToList();
            var newJobs = new List<Job>();
            foreach (var domain in domains)
            {
                var jobsOnDomain = _context.Jobs.Where(b => b.IsCompleted == false && b.DomainId == domain.Id).ToList();
                if (jobsOnDomain.Count == 0)
                {
                    var newJob = new Job
                    {
                        Domain = domain,
                        DomainId = domain.Id,
                        Task = JobName.SignDomain,
                        CreatedAt = DateTime.Now,
                        RunAfter = DateTime.Now
                    };
                    newJobs.Add(newJob);
                }
            }
            _context.Jobs.AddRange(newJobs);
            _context.SaveChanges();
            return "success";
        }

        [HttpPost]
        public string RolloverAllDomains()
        {
            var domains = _context.Domains.Where(b => b.SignMatch).ToList();
            var newJobs = new List<Job>();
            foreach (var domain in domains)
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
                    newJobs.Add(newJob);
                }
            }
            _context.Jobs.AddRange(newJobs);
            _context.SaveChanges();
            return "success";
        }

        [HttpPost]
        public string UnsignAllDomains()
        {
            var domains = _context.Domains.Where(b => b.SignMatch).ToList();
            var newJobs = new List<Job>();
            foreach (var domain in domains)
            {
                var jobsOnDomain = _context.Jobs.Where(b => b.IsCompleted == false && b.DomainId == domain.Id).ToList();
                if (jobsOnDomain.Count == 0)
                {
                    var newJob = new Job
                    {
                        Domain = domain,
                        DomainId = domain.Id,
                        Task = JobName.UnSignDomain,
                        CreatedAt = DateTime.Now,
                        RunAfter = DateTime.Now
                    };
                    newJobs.Add(newJob);
                }
            }
            _context.Jobs.AddRange(newJobs);
            _context.SaveChanges();
            return "success";
        }

        // GET: Domains/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var domain = await _context.Domains
                .Include(d => d.DnsServer)
                .Include(d => d.NameServerGroup).ThenInclude(d => d.NameServers)
                .Include(d => d.Registry)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (domain == null)
            {
                return NotFound();
            }

            if (!domain.RemovedFromDnsServer)
            {
                var signJobs =
                    _context.Jobs.Where(b => b.Task == JobName.SignDomain && b.DomainId == domain.Id && !b.IsCompleted).ToList();

                var isSigning = (signJobs.Count > 0);


                var rolloverJobs =
                    _context.Jobs.Where(b => b.Task == JobName.KeyRolloverDomain && b.DomainId == domain.Id && !b.IsCompleted).ToList();

                var isInRollover = (rolloverJobs.Count > 0);

                var unsignJobs =
                    _context.Jobs.Where(b => b.Task == JobName.UnSignDomain && b.DomainId == domain.Id && !b.IsCompleted).ToList();

                var isUnsigning = (unsignJobs.Count > 0);
                IDnsProvider dnsProvider;
                try
                {
                    dnsProvider = _providerDecider.DnsProvider(domain.DnsServer);
                }
                catch (Exception)
                {
                    var returnVm = new DomainsDetailsViewModel
                    {
                        Domain = domain,
                        Rrsets = new List<DnsRrset>(),
                        Dnssecs = new List<DomainDetailsDnssec>(),
                        DomainType = null,
                        MasterIps = new List<string>(),
                        HasDnsConnection = false,
                        IsInRollover = isInRollover,
                        IsSigning = isSigning,
                        IsUnsigning = isUnsigning
                    };

                    return View(returnVm);
                }

                var rrsets = dnsProvider.GetRrsets(domain.Name);
                rrsets = rrsets.OrderBy(b => b.Type != "SOA").ThenBy(b => b.Type).ToList();

                DnsZoneInfo zoneInfo;
                try
                {
                    zoneInfo = dnsProvider.GetZoneInfo(domain.Name);
                }
                catch (Exception)
                {
                    var returnVm = new DomainsDetailsViewModel
                    {
                        Domain = domain,
                        Rrsets = new List<DnsRrset>(),
                        Dnssecs = new List<DomainDetailsDnssec>(),
                        DomainType = null,
                        MasterIps = new List<string>(),
                        HasDnsConnection = true,
                        HasDnsInfo = false,
                        IsInRollover = isInRollover,
                        IsSigning = isSigning,
                        IsUnsigning = isUnsigning
                    };

                    return View(returnVm);
                }

                var dnsServerDnssecs = zoneInfo.DnsZoneCryptokeys;

                var dnssecs = dnsServerDnssecs.Select(
                    dnsServerDnssec => new DomainDetailsDnssec
                    {
                        Key = dnsServerDnssec.Key,
                        Algo = dnsServerDnssec.Algo,
                        Flag = dnsServerDnssec.Flag,
                        OnDns = true
                    }
                ).ToList();

                if (domain.CustomRegistryId != null)
                {
                    var registryProvider = _providerDecider.InitializeRegistryProvider(domain.Registry);
                    var registryDomainInfo = registryProvider.GetDomainInfo(domain.Name);
                    var registryDnssecs = registryDomainInfo.RegistryDnsSecs;
                    registryProvider.Close();
                    if (registryDnssecs != null)
                    {
                        foreach (var registryDnssec in registryDnssecs)
                        {
                            var checkMatch = dnssecs.FirstOrDefault(b =>
                                b.Key == registryDnssec.Key && b.Flag == registryDnssec.Flag &&
                                b.Algo == registryDnssec.Algo);
                            if (checkMatch != null)
                            {
                                checkMatch.OnRegistry = true;
                            }
                            else
                            {
                                var newDnssec = new DomainDetailsDnssec
                                {
                                    Key = registryDnssec.Key,
                                    Algo = registryDnssec.Algo,
                                    Flag = registryDnssec.Flag,
                                    OnRegistry = true
                                };
                                dnssecs.Add(newDnssec);
                            }
                        }
                    }
                }


                var type = zoneInfo.Type.ToString();
                var masterIps = zoneInfo.MasterIps;

                var vm = new DomainsDetailsViewModel
                {
                    Domain = domain,
                    Rrsets = rrsets,
                    Dnssecs = dnssecs,
                    DomainType = type,
                    MasterIps = masterIps,
                    HasDnsConnection = true,
                    HasDnsInfo = true,
                    IsSigning = isSigning,
                    IsInRollover = isInRollover,
                    IsUnsigning = isUnsigning
                };

                return View(vm);
            }
            else
            {
                var vm = new DomainsDetailsViewModel
                {
                    Domain = domain,
                    Rrsets = new List<DnsRrset>(),
                    Dnssecs = new List<DomainDetailsDnssec>(),
                    DomainType = null,
                    MasterIps = new List<string>(),
                    HasDnsConnection = true,
                    HasDnsInfo = true
                };

                return View(vm);
            }
        }

        [HttpPost]
        public string DeleteRrset(int id, string name, string type, string content)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.DeleteRrset(domain.Name, name, type, content);

            if (!resp.Success)
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string EditRrset(int id, string oldName, int oldTtl, string oldType, string oldContent, string newName, int newTtl, string newContent, bool replace = false)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.EditRrset(domain.Name, oldName, oldTtl, oldType, oldContent, newName, newTtl, newContent, replace);

            if (!resp.Success || resp.Error == "ttl replaced")
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string AddRrset(int id, string name, int ttl, string type, string content, bool replace = false)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.CreateRrset(domain.Name, name, ttl, type, content, replace);

            if (!resp.Success || resp.Error == "ttl replaced")
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string Notify(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.NotifyZone(domain.Name);

            if (!resp.Success)
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string Retrieve(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.RetrieveZone(domain.Name);

            if (!resp.Success)
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string Verify(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.VerifyZone(domain.Name);

            if (!resp.Success)
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string Rectify(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.RectifyZone(domain.Name);

            if (!resp.Success)
            {
                return resp.Error;
            }

            return "success";
        }

        [HttpPost]
        public string Sign(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            if (domain.CustomRegistryId == null)
            {
                return "This domain doesn't have a registry, you are unable to sign this domain";
            }

            if (domain.SignMatch)
            {
                return "This domain is already signed";
            }

            var jobsOnDomain = _context.Jobs.Where(b => b.DomainId == id && !b.IsCompleted).ToList();

            if (jobsOnDomain.Count > 0)
            {
                return "There are already jobs on this domain";
            }
            _context.Jobs.Add(new Job
            {
                Task = JobName.SignDomain,
                DomainId = id,
                Domain = domain,
                CreatedAt = DateTime.Now,
                RunAfter = DateTime.Now
            });
            _context.SaveChanges();
            return "success";
        }

        [HttpPost]
        public string Unsign(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            //if (!domain.SignMatch)
            //{
            //    return "Domain isn't signed";
            //}
            var jobsOnDomain = _context.Jobs.Where(b => b.DomainId == id && !b.IsCompleted).ToList();

            if (jobsOnDomain.Count > 0)
            {
                return "There is already a running job for this domain";
            }
            _context.Jobs.Add(new Job
            {
                Task = JobName.UnSignDomain,
                DomainId = id,
                Domain = domain,
                CreatedAt = DateTime.Now,
                RunAfter = DateTime.Now
            });
            _context.SaveChanges();

            return "success";
        }

        [HttpPost]
        public string Rollover(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            if (domain.CustomRegistryId == null)
            {
                return "This domain doesn't have a registry, you are unable to rollover this domain";
            }
            var jobsOnDomain = _context.Jobs.Where(b => b.DomainId == id && !b.IsCompleted).ToList();

            if (jobsOnDomain.Count > 0)
            {
                return "There are already jobs on this domain";
            }
            _context.Jobs.Add(new Job
            {
                Task = JobName.KeyRolloverDomain,
                DomainId = id,
                Domain = domain,
                CreatedAt = DateTime.Now,
                RunAfter = DateTime.Now
            });
            _context.SaveChanges();


            return "success";
        }

        [HttpPost]
        public string EditKind(int id, string kind)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider dnsProvider;
            try
            {
                dnsProvider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            dnsProvider.ChangeKind(domain.Name, kind);

            return "success";
        }

        [HttpPost]
        public string EditIps(int id, string ips)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider dnsProvider;
            try
            {
                dnsProvider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            try
            {
                var ipsList = ips.Split(',').ToList<string>();
                ipsList = ipsList.Select(t => t.Trim()).ToList();

                var resp = dnsProvider.EditMasterIps(domain.Name, ipsList);

                if (!resp.Success)
                {
                    return resp.Error;
                }
            }
            catch (Exception)
            {
                var resp = dnsProvider.EditMasterIps(domain.Name, new List<string> { "" });

                if (!resp.Success)
                {
                    return resp.Error;
                }
            }

            return "success";
        }
        [HttpPost]
        public string DeleteDomainFromDns(int id)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(domain.DnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var resp = provider.DeleteZone(domain.Name);

            if (!resp.Success)
            {
                return resp.Error;
            }

            domain.RemovedFromDnsServer = true;
            _context.SaveChanges();

            return "success";
        }

        [HttpPost]
        public string DeleteDomainFromDb(int id)
        {
            var domain = _context.Domains.FirstOrDefault(b => b.Id == id);

            if (domain == null)
            {
                return "Domain was not found";
            }

            RemoveDomainFromDatabase(domain);

            return "success";
        }

        [HttpPost]
        public string CreateDomain(int dnsId, string name)
        {
            name = name.ToLower();
            var dnsServer = _context.DnsServers.FirstOrDefault(b => b.Id == dnsId);

            IDnsProvider provider;
            try
            {
                provider = _providerDecider.DnsProvider(dnsServer);
            }
            catch (Exception)
            {
                return "Connection with the DNS Server failed.";
            }

            var createResponse = provider.CreateZone(name);

            if (!createResponse.Success)
            {
                return createResponse.Error;
            }

            var newDomain = new Domain
            {
                Name = name,
                CreatedAt = DateTime.Now,
                DnsServerId = dnsId,
                TopLevelDomainId = _utilities.GetTldId(name),
                Ttl = provider.GetTtl(name)
            };
            _context.Add(newDomain);
            _context.SaveChanges();
            // TODO: DELETE The domain here
            return "success: " + newDomain.Id;
        }

        [HttpPost]
        public string CheckDomainNow(int id)
        {
            var domain = _context.Domains
                .Where(d => d.Id == id)
                .Include(d => d.Registry)
                .Include(d => d.DnsServer)
                .FirstOrDefault();

            if (domain == null)
            {
                return "Domain was not found";
            }

            var listOfDomains = new List<Domain> { domain };
            //var listOfRegistries = new List<Registry> { domain.Registry };
            //var listOfIRegistryProviders = new List<IRegistryProvider> { _providerDecider.InitializeRegistryProvider(domain.Registry) };

            // Initialize Registries
            //var initializedRegistries = new List<Registry>();
            //var registriesFromDb = _context.Registries.ToList();
            //foreach (var registry in registriesFromDb)
            //{
            //    registry.RegistryProvider = _providerDecider.InitializeRegistryProvider(registry);
            //    initializedRegistries.Add(registry);
            //}

            // OLD METHOD:
            List<IRegistryProvider> registryProviders = new List<IRegistryProvider>();
            List<Registry> registries = new List<Registry>();
            var registriesFromDb = _context.Registries.ToList();
            foreach (var registry in registriesFromDb)
            {
                //NOTE: We need connections to all registries in case this domain moved to another one
                IRegistryProvider provider;
                try
                {
                    provider = _providerDecider.InitializeRegistryProvider(registry);
                }
                catch (Exception e)
                {
                    _context.Logs.Add(new Log
                    {
                        Message = e.Message,
                        LogType = LogType.Error,
                        RegistryId = registry.Id,
                        CreatedAt = DateTime.Now
                    });

                    _context.SaveChanges();
                    continue;
                }
                registryProviders.Add(provider);
                registries.Add(registry);
            }

            var checkDomain = new CheckDomain(_context, _utilities, _globals, new Job(), registryProviders, registries);
            checkDomain.CheckListOfDomains(listOfDomains);

            // Close connections
            foreach (var registry in registryProviders)
            {
                try
                {
                    registry.Close();
                }
                catch (Exception)
                {
                    _context.Logs.Add(Logging.LogGeneral(LogType.Error, "Check Now connection could not be closed with a registry ... "));
                }
            }

            return "success";

        }

        [HttpPost]
        public string ChangeExcludeSigning(int domainId, bool excludeSigning)
        {
            var domain = _context.Domains.Include(b => b.DnsServer).FirstOrDefault(b => b.Id == domainId);

            if (domain == null)
            {
                return "Domain was not found";
            }

            domain.ExcludeSigning = excludeSigning;

            _context.SaveChanges();

            return "success";
        }

        private void RemoveDomainFromDatabase(Domain domain)
        {
            var logs = _context.Logs.Where(b => b.DomainId == domain.Id).ToList();
            foreach (var log in logs)
            {
                log.DomainId = null;
                log.Domain = null;
            }

            var jobs = _context.Jobs.Where(b => b.DomainId == domain.Id).ToList();
            foreach (var job in jobs)
            {
                job.IsCompleted = true;
                job.IsSuccessful = false;
                job.DomainId = null;
                job.Domain = null;
            }

            var cryptokeys = _context.Cryptokeys.Where(b => b.DomainId == domain.Id).ToList();
            _context.RemoveRange(cryptokeys);

            _context.Remove(domain);
            _context.SaveChanges();
        }
    }
}
