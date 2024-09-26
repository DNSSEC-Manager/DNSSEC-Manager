using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Providers.Providers.Transip.Dto
{
    class DnssecEntry
    {
        [JsonProperty("keyTag")]
        public string KeyTag { get; set; }

        [JsonProperty("flags")]
        public string Flags { get; set; }

        [JsonProperty("algorithm")]
        public string Algorithm { get; set; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }
    }
}
