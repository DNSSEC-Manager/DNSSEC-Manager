using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Providers.Providers.Transip.Dto
{
    class Nameservers
    {
        [JsonProperty("nameservers")]
        public List<Nameserver> NameserverList { get; set; }
    }
}
