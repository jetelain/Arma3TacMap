using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;

namespace Arma3TacMapWebApp.Models
{
    public class ImportLayerViewModel
    {
        public TacMap TacMap { get; set; }

        [Required]
        public string Label { get; set; }
        public TacMapAccess Access { get; internal set; }
    }
}
