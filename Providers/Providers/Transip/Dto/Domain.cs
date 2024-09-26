using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Providers.Providers.Transip.Dto
{
    class Domain
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        //[JsonProperty("authCode")]
        //public string AuthCode { get; set; }
        //[JsonProperty("isTransferLocked")]
        //public bool IsTransferLocked { get; set; }
        //[JsonProperty("registrationDate")]
        //public string RegistrationDate { get; set; }
        //[JsonProperty("renewalDate")]
        //public string RenewalDate { get; set; }
        //[JsonProperty("isWhitelabel")]
        //public bool IsWhitelabel { get; set; }
        //[JsonProperty("cancellationDate")]
        //public string CancellationDate { get; set; }
        //[JsonProperty("cancellationStatus")]
        //public string CancellationStatus { get; set; }
        [JsonProperty("isDnsOnly")]
        public bool IsDnsOnly { get; set; }
        //[JsonProperty("tags")]
        //public string[] Tags { get; set; }

    }
}
