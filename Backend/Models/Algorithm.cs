using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Algorithm
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Bits { get; set; }

        public bool IsDefault { get; set; }

        public bool Disabled { get; set; }
    }
}
