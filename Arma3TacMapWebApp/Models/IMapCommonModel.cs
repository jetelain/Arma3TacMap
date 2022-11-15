namespace Arma3TacMapWebApp.Models
{
    public interface IMapCommonModel
    {
        string endpoint { get;  }

        string worldName { get; }

        bool isReadOnly { get; }
        string init { get; }
        string view { get; }
    }
}