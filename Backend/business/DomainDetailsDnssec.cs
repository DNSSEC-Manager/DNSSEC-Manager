using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Business
{
    public class DomainDetailsDnssec
    {

        public string Flag { get; set; }

        public string Algo { get; set; }

        public string Key { get; set; }

        public bool OnDns { get; set; }

        public bool OnRegistry { get; set; }
    }
}
