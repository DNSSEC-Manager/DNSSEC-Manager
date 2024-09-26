using Newtonsoft.Json;
using System.Collections.Generic;

namespace Providers.Powerdns.Dto
{

    public class CryptokeysJson
    {
        public bool active { get; set; }
        public string algorithm { get; set; }
        public int bits { get; set; }
        public string dnskey { get; set; }
        public List<string> ds { get; set; }
        public int flags { get; set; }
        public int id { get; set; }
        public string keytype { get; set; }
        public string type { get; set; }
    }
}
