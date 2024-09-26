using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Providers.Providers.Transip.Dto
{
    class TokenRequest
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("read_only")]
        public bool IsReadOnlyEnabled { get; set; }
        [JsonProperty("expiration_time")]
        public string ExpirationTime { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("global_key")]
        public bool IsGlobalKey { get; set; }
    }
}
