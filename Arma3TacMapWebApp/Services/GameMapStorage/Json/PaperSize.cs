using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public enum PaperSize
    {
        [Display(Name = "A4 (210 x 297 mm)")]
        A4,

        [Display(Name = "A3 (297 x 420 mm)")]
        A3,

        [Display(Name = "A2 (420 x 594 mm)")]
        A2,

        [Display(Name = "A1 (594 x 841 mm)")]
        A1,

        [Display(Name = "A0 (841 x 1189 mm)")]
        A0,

        [Display(Name = "Arch E (914 x 1220 mm)")]
        ArchE
    }
}