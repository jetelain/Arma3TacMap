using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Entities
{
    public enum MessageTemplateType
    {
        Generic,
        
        [Display(Name = "Medical Evacuation")]
        MedicalEvacuation,
        
        [Display(Name = "Artillery")]
        Artillery,

        [Display(Name = "Air Support")]
        AirSupport
    }
}