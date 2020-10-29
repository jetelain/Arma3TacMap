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
    }
}