using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapLibrary.ViewComponents
{
    public class StaticMapModel
    {
        public bool fullScreen { get; set; }

        public double[] center { get; set; }

        public Dictionary<string,MarkerData> markers { get; set; }

        public string endpoint { get; set; }

        public string worldName { get; set; }
    }
}