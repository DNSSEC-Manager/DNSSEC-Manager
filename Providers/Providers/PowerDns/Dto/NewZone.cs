using System.Collections.Generic;
using Newtonsoft.Json;

namespace Providers.Powerdns.Dto
{
    public class NewZone
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("nameservers")]
        public List<string> Nameservers { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
