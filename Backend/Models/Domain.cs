using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Domain
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int DnsServerId { get; set; }
        public DnsServer DnsServer { get; set; }

        public int? CustomRegistryId { get; set; }
        [ForeignKey("CustomRegistryId")]
        public Registry Registry { get; set; }

        public int? NameServerGroupId { get; set; }
        public NameServerGroup NameServerGroup { get; set; }

        public bool RemovedFromDnsServer { get; set; }

        public bool SignMatch { get; set; }

        public bool ExcludeSigning { get; set; }

        public bool IsReservedByScheduler { get; set; }

        public int Ttl { get; set; }

        public int? TopLevelDomainId { get; set; }
        public TopLevelDomain TopLevelDomain { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SignedAt { get; set; }

        public DateTime? LastChecked { get; set; }

        public ICollection<Job> Jobs { get; set; }
        public ICollection<DnsRecordSet> DnsRecordSets { get; set; }
        public ICollection<Cryptokey> Cryptokeys { get; set; }
        
        /// <summary>
        /// Gets the domain name without the TLD extension.
        /// For example: "example.com" -> "example"
        /// </summary>
        [NotMapped]
        public string NameWithoutTld
        {
            get
            {
                if (string.IsNullOrEmpty(Name) || TopLevelDomain == null || string.IsNullOrEmpty(TopLevelDomain.Tld))
                {
                    return Name;
                }

                // Remove the TLD and the dot separator
                var tldWithDot = "." + TopLevelDomain.Tld;
                if (Name.EndsWith(tldWithDot, StringComparison.OrdinalIgnoreCase))
                {
                    return Name.Substring(0, Name.Length - tldWithDot.Length);
                }

                return Name;
            }
        }
    }
}
