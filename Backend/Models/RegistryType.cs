using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public enum RegistryType
    {
        [Display(Name = "SIDN")]
        Sidn = 1,
        [Display(Name = "Openprovider")]
        Openprovider = 2,
        [Display(Name = "TransIP")]
        Transip = 3,
        [Display(Name = "EURid")]
        Eurid = 4
    }
}
