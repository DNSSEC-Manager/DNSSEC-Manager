using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Providers.Providers.Openprovider.Dto
{
    [XmlRoot(ElementName = "modifyDomainRequest")]
    public class ModifyDomainRequest
    {
        [XmlElement("domain")]
        public Domain Domain { get; set; }

        [XmlElement("isDnssecEnabled")]
        public int IsDnssecEnabled { get; set; }

        [XmlElement("dnssecKeys")]
        public ModifyDomainDnsseckeys Dnsseckeys { get; set; }
    }
}
