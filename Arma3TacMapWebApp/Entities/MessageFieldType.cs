using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Entities
{
    public enum MessageFieldType
    {
        [Display(Name = "Text")]
        Text,

        [Display(Name = "Number (Integer)")]
        Number,

        [Display(Name = "Date and time (Local)")]
        DateTime,

        [Display(Name = "Call sign")]
        CallSign,

        [Display(Name = "Frequency (MHz)")]
        Frequency,

        [Display(Name = "Grid position (MRGS coordinates)")]
        Grid,

        [Display(Name = "Checkbox")]
        CheckBox
    }
}