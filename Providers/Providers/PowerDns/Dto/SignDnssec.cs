using Newtonsoft.Json;

namespace Providers.Powerdns.Dto
{
    public class SignDnssec
    {
        [JsonProperty("dnssec")]
        public bool Dnssec;
        [JsonProperty("keytype")]
        public string Keytype;
        [JsonProperty("active")]
        public bool Active;
        [JsonProperty("bits")]
        public int Bits;
        [JsonProperty("algorithm")]
        public string Algorithm;
    }
}