using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class StaticMapModel : IMapCommonModel
    {
        public double[] center { get; set; }

        public Dictionary<string,MarkerData> markers { get; set; }

        public string endpoint { get; set; }

        public string worldName { get; set; }

        public bool fullScreen { get; set; }

        bool IMapCommonModel.isReadOnly => true;

        string IMapCommonModel.init => "initStaticMap";
    }
}