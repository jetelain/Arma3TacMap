using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapLibrary.Arma3
{
    public enum Arma3MarkerColor
    {
        [Display(Name = "Noir")]
        ColorBlack,
        [Display(Name = "Gris")]
        ColorGrey,
        [Display(Name = "Rouge")]
        ColorRed,
        [Display(Name = "Marron")]
        ColorBrown,
        [Display(Name = "Orange")]
        ColorOrange,
        [Display(Name = "Jaune")]
        ColorYellow,
        [Display(Name = "Khaki")]
        ColorKhaki,
        [Display(Name = "Vert")]
        ColorGreen,
        [Display(Name = "Bleu")]
        ColorBlue,
        [Display(Name = "Rose")]
        ColorPink,
        [Display(Name = "Blanc")]
        ColorWhite,
        [Display(Name = "Inconnu")]
        ColorUNKNOWN,
        [Display(Name = "BLUFOR")]
        colorBLUFOR,
        [Display(Name = "OPFOR")]
        colorOPFOR,
        [Display(Name = "Indépendent")]
        colorIndependent,
        [Display(Name = "Civil")]
        colorCivilian
    }
}
