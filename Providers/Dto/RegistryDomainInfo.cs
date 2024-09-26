using System.Collections.Generic;

namespace Providers.Dto
{
    public class RegistryDomainInfo
    {
        public string Name { get; set; }

        public string Error { get; set; }

        public List<string> NameServers { get; set; }

        public ICollection<RegistryDnsSec> RegistryDnsSecs { get; set; }
    }
}
