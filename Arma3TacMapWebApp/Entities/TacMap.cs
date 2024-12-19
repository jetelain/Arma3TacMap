using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMap
    {
        public int TacMapID { get; set; }

        [Display(Name = "Owner")]
        public int OwnerUserID { get; set; }
        [Display(Name = "Owner")]
        public User? Owner { get; set; }

        [Display(Name = "Creation date")]
        public DateTime Created { get; set; }

        [Display(Name = "Label")]
        [Required]
        public string Label { get; set; }

        public string? ReadOnlyToken { get; set; }

        public string? ReadWriteToken { get; set; }

        [Display(Name = "Map background")]
        [Required]
        public string WorldName { get; set; }

        [Display(Name = "Game")]
        public string GameName { get; set; }

        public Uri? EventHref { get; set; }

        [NotMapped]
        public GameMapJsonBase? MapInfos { get; set; }

        public int? ParentTacMapID { get; set; }

        public TacMap? Parent { get; set; }


        [Display(Name = "Friendly ORBAT")]
        public int? FriendlyOrbatID { get; set; }
        [Display(Name = "Friendly ORBAT")]
        public Orbat? FriendlyOrbat { get; set; }

        [Display(Name = "Hostile ORBAT")]
        public int? HostileOrbatID { get; set; }
        [Display(Name = "Hostile ORBAT")]
        public Orbat? HostileOrbat { get; set; }
    }
}
