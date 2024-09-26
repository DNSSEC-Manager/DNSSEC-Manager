using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Providers.Providers.Openprovider.Dto
{
    [XmlRoot(ElementName = "credentials")]
    public class Credentials
    {
        [XmlElement("username")]
        public string Username { get; set; }
        [XmlElement("password")]
        public string Password { get; set; }
    }
}
