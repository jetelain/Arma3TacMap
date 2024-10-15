using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public enum PaperFileFormat
    {
        [Display(Name = "Full Page")]
        SinglePDF,

        [Display(Name = "Booklet")]
        BookletPDF
    }
}