using Backend.Models;
using System.Collections.Generic;

namespace Backend.ViewModels
{
    public class DashboardViewModel
    {
        public int DomainCountTotal { get; set; }
        public int DomainCountRemovedFromDnsServer { get; set; }
        public int DomainCountHasRegistry { get; set; }
        public int DomainCountNameserversMatch { get; set; }
        public int DomainCountDnssecSigned { get; set; }

        // used by diagrams:
        public int SignedDomainsCount { get; set; }
        public int NotSignedDomainsCount { get; set; }
        public int ActiveDomainsCount { get; set; }
        public int InActiveDomainsCount { get; set; }
        public int RemovedDomainsCount { get; set; }

        public IEnumerable<TopLevelDomain> TopLevelDomains { get; set; }
        public IEnumerable<Domain> LastAddedDomains { get; set; }


    }
}
