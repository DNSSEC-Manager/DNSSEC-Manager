using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Cryptokey
    {
        public int Id { get; set; }

        public string Flag { get; set; }

        public string Algo { get; set; }

        public string Key { get; set; }

        public string KeyTag { get; set; }

        public int DomainId { get; set; }
        [ForeignKey("DomainId")]
        public virtual Domain Domain { get; set; }

        public ICollection<Job> Jobs { get; set; }
    }
}
