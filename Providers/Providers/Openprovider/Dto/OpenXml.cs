using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Providers.Providers.Openprovider.Dto
{
    [XmlRoot(ElementName = "openXML")]
    public class OpenXml
    {
        [XmlElement("credentials")]
        public Credentials Credentials { get; set; }

        [XmlElement("retrieveDomainRequest")]
        public RetrieveDomainRequest RetrieveDomainRequest { get; set; }

        [XmlElement("modifyDomainRequest")]
        public ModifyDomainRequest ModifyDomainRequest { get; set; }
    }
}
