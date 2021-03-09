using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arma3TacMapLibrary.Arma3;

namespace Arma3TacMapWebApp.Entities
{
    public class ReplayMap
    {
        public int ReplayMapID { get; set; }

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

        [Display(Name = "Map background")]
        [Required]
        public string WorldName { get; set; }

        [NotMapped]
        public MapInfos MapInfos { get; set; }

        public int? TacMapID { get; set; }

        public TacMap TacMap { get; set; }

        public List<ReplayFrame> Frames { get; set; }

        public List<ReplayPlayer> Players { get; set; }

        public List<ReplayGroup> Groups { get; set; }

        public List<ReplayUnit> Units { get; set; }

        public List<ReplayVehicle> Vehicles { get; set; }
    }
}
