using System;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models;

namespace Backend.Models
{
    public class Log
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string RawMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        public LogType LogType { get; set; }

        public int? DomainId { get; set; }
        [ForeignKey("DomainId")]
        public virtual Domain Domain { get; set; }

        public int? DnsServerId { get; set; }
        [ForeignKey("DnsServerId")]
        public virtual DnsServer DnsServer { get; set; }

        public int? RegistryId { get; set; }
        [ForeignKey("RegistryId")]
        public virtual Registry Registry { get; set; }

        public int? JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
    }
}
