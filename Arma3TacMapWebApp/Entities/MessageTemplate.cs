using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Arma3TacMapWebApp.Entities
{
    public class MessageTemplate
    {
        public int MessageTemplateID { get; set; }

        [Display(Name = "Owner")]
        public int OwnerUserID { get; set; }

        [Display(Name = "Owner")]
        public User? Owner { get; set; }

        [Display(Name = "Creation date")]
        public DateTime Created { get; set; }

        [Display(Name = "Label")]
        [Required]
        public required string Label { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Visibility")]
        public OrbatVisibility Visibility { get; set; }

        [Display(Name = "Type")]
        public MessageTemplateType Type { get; set; }

        [Display(Name = "Country")]
        public string? CountryCode { get; set; }

        public string? Token { get; set; }

        public List<MessageLineTemplate>? Lines { get; set; }

    }
}
