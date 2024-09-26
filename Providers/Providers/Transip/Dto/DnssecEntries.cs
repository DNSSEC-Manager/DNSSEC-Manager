using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Providers.Providers.Transip.Dto
{
    class DnssecEntries
    {
        [JsonProperty("dnsSecEntries")]
        public List<DnssecEntry> DnssecEntryList { get; set; }
    }
}