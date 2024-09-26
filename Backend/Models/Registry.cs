using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Providers;

namespace Backend.Models
{

    public class Registry
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Url { get; set; }

        public string Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool IsDefault { get; set; }

        public RegistryType RegistryType { get; set; }

        //[NotMapped]
        //public IRegistryProvider RegistryProvider { get; set; }

        public ICollection<TopLevelDomainRegistry> TopLevelDomainRegistries { get; } = new List<TopLevelDomainRegistry>();

        public ICollection<Domain> Domains { get; set; }

    }
}
