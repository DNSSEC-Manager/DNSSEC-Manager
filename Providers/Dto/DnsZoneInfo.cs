using System.Collections.Generic;
using Providers.Dto;

namespace Providers.Dto
{
    public enum DnsZoneTypes
    {
        Master = 1,
        Slave = 2,
        Native = 3
    }

    public class DnsZoneInfo
    {
        public string Name { get; set; }

        public DnsZoneTypes Type { get; set; }

        public List<string> MasterIps { get; set; }

        public ICollection<DnsZoneCryptokey> DnsZoneCryptokeys { get; set; }
    }
}