using Newtonsoft.Json;
using Providers.Dto;

namespace Providers.Powerdns.Dto
{
    public class ZoneInfo
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("api_rectify")]
        public bool ApiRectify { get; set; }
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
        [JsonProperty("nsec3narrow")]
        public bool Nsec3Narrow { get; set; }
        [JsonProperty("nsec3param")]
        public string Nsec3Param { get; set; }
        [JsonProperty("rrsets")]
        public DnsRrset[] Rrsets { get; set; }
        [JsonProperty("serial")]
        public int Serial { get; set; }
        [JsonProperty("soa_edit")]
        public string SoaEdit { get; set; }
        [JsonProperty("soa_edit_api")]
        public string SoaEditApi { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}