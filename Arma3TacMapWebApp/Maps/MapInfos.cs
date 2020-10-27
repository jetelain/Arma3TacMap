using System.Collections.Generic;

namespace Arma3TacMapWebApp.Maps
{
    public class MapInfos
	{
		public string tilePattern { get; set; }
		public int maxZoom { get; set; }
		public int minZoom { get; set; }
		public int defaultZoom { get; set; }
		public string attribution { get; set; }
		public int tileSize { get; set; }
		public List<int> center { get; set; }
		public int worldSize { get; set; }
		public string preview { get; set; }
		public string dlc { get; set; }
		public string steamWorkshop { get; set; }
		public List<CityInfos> cities { get; set; }
	}

}
