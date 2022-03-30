namespace Arma3TacMapWebApp.Models
{
    public class LiveMapModel : IMapCommonModel
    {
        public string hub { get; set; }

        public string endpoint { get; set; }

        public object mapId { get; set; }

        public string worldName { get; set; }

        public bool isReadOnly { get; set; }

        string IMapCommonModel.init => "initLiveMap";
    }
}