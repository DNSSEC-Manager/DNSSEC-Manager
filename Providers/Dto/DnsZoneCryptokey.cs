namespace Providers.Dto
{
    public class DnsZoneCryptokey
    {
        public string Flag { get; set; } // Flags

        public string Algo { get; set; } // Algorithm

        public string Key { get; set; } // Public Key

        public int Id { get; set; } // PowerDNS database id

        public string KeyTag { get; set; }
    }
}