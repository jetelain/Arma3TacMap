using System.ComponentModel.DataAnnotations;
using Arma3TacMapWebApp.Entities;

namespace Arma3TacMapWebApp.Models
{
    public class MessageTemplateImportViewModel
    {
        [Display(Name = "Json")]
        [Required]
        public string? Json { get; set; }

        [Display(Name = "Visibility")]
        public OrbatVisibility Visibility { get; set; }
    }
}