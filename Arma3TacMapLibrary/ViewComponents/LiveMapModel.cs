namespace Arma3TacMapLibrary.ViewComponents
{
    public class LiveMapModel
    {
        internal string hub;

        public string endpoint { get; set; }

        public object mapId { get; set; }

        public string worldName { get; set; }

        public bool isReadOnly { get; set; }
    }
}