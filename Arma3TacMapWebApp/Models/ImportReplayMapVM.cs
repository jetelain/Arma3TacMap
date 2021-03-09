using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Arma3TacMapWebApp.Models
{
    public class ImportReplayMapVM
    {
        [Display(Name = "Label")]
        public string Label { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; }

        [Required]
        [Display(Name = "File format")]
        public ImportFileFormat FileFormat { get; set; }
    }
}
