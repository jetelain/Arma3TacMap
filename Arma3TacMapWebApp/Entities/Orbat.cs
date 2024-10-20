using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Entities
{
    public class Orbat
    {
        public int OrbatID { get; set; }

        [Display(Name = "Owner")]
        public int OwnerUserID { get; set; }
        [Display(Name = "Owner")]
        public User? Owner { get; set; }

        [Display(Name = "Creation date")]
        public DateTime Created { get; set; }

        [Display(Name = "Label")]
        [Required]
        public string Label { get; set; }

        [Display(Name = "Visibility")]
        public OrbatVisibility Visibility { get; set; }

        public List<OrbatUnit> Units { get; set; }

        public string? Token { get; set; }
    }
}
