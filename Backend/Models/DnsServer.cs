using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace Backend.Models
{

    public class DnsServer
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string BaseUrl { get; set; }

        [Required]
        public DnsServerType ServerType { get; set; }

        [Required]
        public string AuthToken { get; set; }

        public ICollection<Job> Jobs { get; set; }
        public ICollection<Domain> Domains { get; set; }
        public ICollection<NameServerGroup> NameServerGroups { get; set; }
        
    }
}
