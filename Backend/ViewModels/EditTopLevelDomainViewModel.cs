using Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    public class EditTopLevelDomainViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Tld { get; set; }

        [Required]
        public int? OverrideAlgorithmId { get; set; }

        public IEnumerable<Algorithm> Algorithms { get; set; }
    }
}
