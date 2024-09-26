using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Backend.Models
{
    public class NameServerGroup
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "DNS Server")]
        public int? DnsServerId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [ForeignKey("DnsServerId")]
        [Display(Name = "DNS Server")]
        public DnsServer DnsServer { get; set; }

        public ICollection<NameServer> NameServers { get; set; }

        public ICollection<Domain> Domains { get; set; }

    }
}
