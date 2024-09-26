using System.Collections.Generic;
using System.Xml.Serialization;

namespace Providers.Providers.Openprovider.Dto
{
    [XmlRoot(ElementName = "dnssecKeys")]
    public class ModifyDomainDnsseckeys
    {
        [XmlElement("array")]
        public ModifyDomainSubArray ModifyDomainSubArray { get; set; }

    }

    [XmlRoot(ElementName = "array")]
    public class ModifyDomainSubArray
    {
        [XmlElement("item")]
        public List<ModifyDomainKey> ModifyDomainKeys { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class ModifyDomainKey
    {
        [XmlElement("flags")]
        public string Flags { get; set; }

        [XmlElement("alg")]
        public string Alg { get; set; }

        [XmlElement("protocol")]
        public string Protocol { get; set; }

        [XmlElement("pubKey")]
        public string PubKey { get; set; }
    }
}


