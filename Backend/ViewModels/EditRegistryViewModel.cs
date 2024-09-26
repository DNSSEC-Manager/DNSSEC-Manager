using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.ViewModels
{
    public class EditRegistryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string Port { get; set; }

        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

        [Required]
        public RegistryType RegistryType { get; set; }
    }
}
