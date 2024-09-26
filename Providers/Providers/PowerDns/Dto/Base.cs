using Newtonsoft.Json;

namespace Providers.Powerdns.Dto
{
    public class Base
    {
        [JsonProperty("config_url")]
        public string ConfigUrl { get; set; }
        [JsonProperty("daemon_type")]
        public string DaemonType { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("zones_url")]
        public string ZonesUrl { get; set; }

        public string Error { get; set; }
    }
}
