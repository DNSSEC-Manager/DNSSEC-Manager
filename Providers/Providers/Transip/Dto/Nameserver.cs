using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Providers.Providers.Transip.Dto
{
    class Nameserver
    {
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
        //[JsonProperty("ipv4")]
        //public string Ipv4 { get; set; }
        //[JsonProperty("ipv6")]
        //public string Ipv6 { get; set; }
    }
}