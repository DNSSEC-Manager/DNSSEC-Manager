using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Providers.Providers.Openprovider.Dto
{
    [XmlRoot(ElementName = "domain")]
    public class Domain
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("extension")]
        public string Extension { get; set; }
    }
}
