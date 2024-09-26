using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Backend.ViewModels
{
    public class EditDnsServerViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string BaseUrl { get; set; }

        [Required]
        public DnsServerType ServerType { get; set; }

        public string AuthToken { get; set; }
        
    }
}
