using Newtonsoft.Json;

namespace Providers.Dto
{
    public class DnsRecord
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }
    }
}
