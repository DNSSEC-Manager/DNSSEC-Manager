using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class DnsRecordSet
    {
        public int Id { get; set; }

        public int DomainId { get; set; }
        public Domain Domain { get; set; }

        public string Records { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastCheckedAt { get; set; }
    }
}
