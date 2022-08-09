using System.Collections.Generic;
using Arma3TacMapWebApp.Entities;

namespace Arma3TacMapWebApp.Models
{
    public class ExportViewModel
    {
        public ExportViewModel()
        {
        }

        public TacMap TacMap { get; set; }
        public string Script { get; internal set; }
        public TacMapAccess Access { get; internal set; }
        public List<TacMap> Layers { get; internal set; }
        public bool IsPartialExport { get; internal set; }
    }
}