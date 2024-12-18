using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Entities
{
    public enum MessageFieldType
    {
        [Display(Name = "Text")]
        Text,

        [Display(Name = "Number (Integer)")]
        Number,

        [Display(Name = "Date and Time (Local)")]
        DateTime,

        [Display(Name = "Call Sign")]
        CallSign,

        [Display(Name = "Frequency (MHz)")]
        Frequency,

        [Display(Name = "Grid Position (MRGS coordinates)")]
        Grid,

        [Display(Name = "Checkbox")]
        CheckBox
    }
}