using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapLibrary.ViewComponents
{
    internal class StaticMapModel
    {
        public double[] center { get; set; }

        public Dictionary<string,MarkerData> markers { get; set; }

        public string endpoint { get; set; }

        public string worldName { get; set; }
    }
}