using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;

namespace Arma3TacMapWebApp.Models
{
    public class StaticMapModel : IMapCommonModel
    {
        public double[] center { get; set; }

        public Dictionary<string,MarkerData> markers { get; set; }

        public string endpoint { get; set; }

        public string worldName { get; set; }

        public bool fullScreen { get; set; }

        public string view { get; set; }

        public required GameJson Game { get; set; }

        public required GameMapJson GameMap { get; set; }

        public required string GmsBaseUri { get; set; }

        bool IMapCommonModel.isReadOnly => true;

        string IMapCommonModel.init => "initStaticMap";
    }
}