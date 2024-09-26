using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Models;
using Providers.Dto;

namespace Backend.ViewModels
{
    public class DomainsDetailsViewModel
    {
        public Domain Domain { get; set; }

        public string DomainType { get; set; }

        public bool HasDnsConnection { get; set; }

        public bool HasDnsInfo { get; set; }

        public List<string> MasterIps { get; set; }

        public List<DnsRrset> Rrsets { get; set; }

        public List<DomainDetailsDnssec> Dnssecs { get; set; }
        
        public bool IsSigning { get; set;}

        public bool IsInRollover { get; set; }

        public bool IsUnsigning { get; set; }
    }
}
