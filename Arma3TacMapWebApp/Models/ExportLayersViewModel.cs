using System.Collections.Generic;
using Arma3TacMapWebApp.Entities;

namespace Arma3TacMapWebApp.Models
{
    public class ExportLayersViewModel
    {
        public ExportLayersViewModel()
        {
        }

        public TacMap TacMap { get; set; }

        public TacMapAccess Access { get; set; }

        public List<TacMap> Layers { get; set; }
    }
}