using Newtonsoft.Json;

namespace Providers.Powerdns.Dto
{
    public class Zone
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("dnssec")]
        public bool Dnssec { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("last_check")]
        public int LastCheck { get; set; }
        [JsonProperty("masters")]
        public object[] Masters { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("notified_serial")]
        public int NotifiedSerial { get; set; }
        //[JsonProperty("serial")]
        //public long Serial { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
