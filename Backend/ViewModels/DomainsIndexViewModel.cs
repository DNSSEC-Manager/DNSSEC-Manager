using System.Collections.Generic;
using Backend.Business;
using Backend.Models;

namespace Backend.ViewModels
{
    public class DomainsIndexViewModel
    {
        public string Search { get; set; }
        public int? RegistryId { get; set; }
        public int? DnsId { get; set; }
        public int? NameserversId { get; set; }
        public string Sort { get; set; }
        public int? Removed { get; set; }
        public int? Signed { get; set; }
        public int? Ttl { get; set; }

        public int SelectionDomainsCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int PageSize { get; set; }

        public bool AutomaticSigningEnabled { get; set; }

        public List<Registry> Registries { get; set; }
        public List<DnsServer> DnsServers { get; set; }
        public List<NameServerGroup> NameServerGroups { get; set; }

        public IEnumerable<Domain> Domains { get; set; }
    }
}
