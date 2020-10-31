namespace Arma3TacMapLibrary.ViewComponents
{
    public class LiveMapModel
    {
        internal string endpoint;

        public object mapId { get; set; }

        public string worldName { get; set; }

        public bool isReadOnly { get; set; }
    }
}