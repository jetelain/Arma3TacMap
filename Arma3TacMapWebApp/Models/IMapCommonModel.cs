using Arma3TacMapWebApp.Services.GameMapStorage.Json;

namespace Arma3TacMapWebApp.Models
{
    public interface IMapCommonModel
    {
        string endpoint { get;  }

        string worldName { get; }

        bool isReadOnly { get; }

        string init { get; }

        string view { get; }

        GameJson Game { get; }

        GameMapJson GameMap { get; }

        string GmsBaseUri { get; }
    }
}