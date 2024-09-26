using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Backend.Models
{

    public class Job
    {
        public int Id { get; set; }

        public int? DnsServerId { get; set; }
        [ForeignKey("DnsServerId")]
        public DnsServer DnsServer { get; set; }

        public int? DomainId { get; set; }
        [ForeignKey("DomainId")]
        public Domain Domain { get; set; }

        public int? CryptokeyId { get; set; }
        [ForeignKey("CryptokeyId")]
        public Cryptokey Cryptokey { get; set; }

        public JobName Task { get; set; }

        public JobStep? Step { get; set; }

        public bool IsPermanent { get; set; }

        public bool IsSuccessful { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime RunAfter { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
