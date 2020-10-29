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

        [Display(Name = "Propriétaire")]
        public int OwnerUserID { get; set; }
        [Display(Name = "Propriétaire")]
        public User Owner { get; set; }

        [Display(Name = "Date de création")]
        public DateTime Created { get; set; }

        [Display(Name = "Libellé")]
        [Required]
        public string Label { get; set; }

        public string ReadOnlyToken { get; set; }

        public string ReadWriteToken { get; set; }

        [Display(Name = "Fond de carte")]
        [Required]
        public string WorldName { get; set; }

        [NotMapped]
        public MapInfos MapInfos { get; set; }

        public int? ParentTacMapID { get; set; }

        public TacMap Parent { get; set; }
    }
}
