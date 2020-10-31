using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapLibrary.Arma3
{
    public enum Arma3MarkerColor
    {
        [Display(Name = "Black")]
        ColorBlack,
        [Display(Name = "Grey")]
        ColorGrey,
        [Display(Name = "Red")]
        ColorRed,
        [Display(Name = "Brown")]
        ColorBrown,
        [Display(Name = "Orange")]
        ColorOrange,
        [Display(Name = "Yellow")]
        ColorYellow,
        [Display(Name = "Khaki")]
        ColorKhaki,
        [Display(Name = "Green")]
        ColorGreen,
        [Display(Name = "Blue")]
        ColorBlue,
        [Display(Name = "Pink")]
        ColorPink,
        [Display(Name = "White")]
        ColorWhite,
        [Display(Name = "UNKNOWN")]
        ColorUNKNOWN,
        [Display(Name = "BLUFOR")]
        colorBLUFOR,
        [Display(Name = "OPFOR")]
        colorOPFOR,
        [Display(Name = "Independent")]
        colorIndependent,
        [Display(Name = "Civilian")]
        colorCivilian
    }
}
