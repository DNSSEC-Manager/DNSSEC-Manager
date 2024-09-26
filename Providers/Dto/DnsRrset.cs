using Newtonsoft.Json;
using Providers.Powerdns.Dto;

namespace Providers.Dto
{
    public class DnsRrset
    {
        [JsonProperty("comments")]
        public object[] Comments { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("records")]
        public DnsRecord[] Records { get; set; }
        [JsonProperty("ttl")]
        public int Ttl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("changetype")]
        public string Changetype { get; set; }
    }
}
