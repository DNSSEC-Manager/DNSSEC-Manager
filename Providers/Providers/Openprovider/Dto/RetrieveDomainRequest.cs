using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Providers.Providers.Openprovider.Dto
{
    [XmlRoot(ElementName = "retrieveDomainRequest")]
    public class RetrieveDomainRequest
    {
        [XmlElement("domain")]
        public Domain Domain { get; set; }

        [XmlElement("withadditionalData")]
        public int WithadditionalData { get; set; }
    }
}
