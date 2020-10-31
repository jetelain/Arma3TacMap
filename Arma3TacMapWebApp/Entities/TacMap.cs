using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Maps;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMap
    {
        public int TacMapID { get; set; }

        [Display(Name = "Owner")]
        public int OwnerUserID { get; set; }
        [Display(Name = "Owner")]
        public User Owner { get; set; }

        [Display(Name = "Creation date")]
        public DateTime Created { get; set; }

        [Display(Name = "Label")]
        [Required]
        public string Label { get; set; }

        public string ReadOnlyToken { get; set; }

        public string ReadWriteToken { get; set; }

        [Display(Name = "Map background")]
        [Required]
        public string WorldName { get; set; }

        [NotMapped]
        public MapInfos MapInfos { get; set; }

        public int? ParentTacMapID { get; set; }

        public TacMap Parent { get; set; }
    }
}
