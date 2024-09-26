using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class TopLevelDomainRegistry
    {
        public int TopLevelDomainId { get; set; }
        public TopLevelDomain TopLevelDomain { get; set; }

        public int RegistryId { get; set; }
        public Registry Registry { get; set; }

    }
}
