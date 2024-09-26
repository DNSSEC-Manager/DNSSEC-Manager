using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class TopLevelDomain
    {
        public int Id { get; set; }

        [Required]
        public string Tld { get; set; }

        public int? OverrideAlgorithmId { get; set; }
        [ForeignKey("OverrideAlgorithmId")]
        public Algorithm Algorithm { get; set; }

        public ICollection<Domain> Domains { get; set; }

        public ICollection<TopLevelDomainRegistry> TopLevelDomainRegistries { get; } = new List<TopLevelDomainRegistry>();
    }
}
