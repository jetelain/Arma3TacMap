using Arma3TacMapWebApp.Services.GameMapStorage.Json;
using System;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Services.GameMapStorage
{
    public interface IGameMapStorageService
    {
        Uri BaseUri { get; }

        Task<GameJsonBase[]> GetGames();

        Task<GameJsonBase?> GetGameBase(string gameNameOrId);

        Task<GameJson?> GetGame(string gameNameOrId);

        Task<GameMapJsonBase[]> GetMaps(string gameNameOrId);

        Task<GameMapJson?> GetMap(string gameNameOrId, string mapNameOrId);

        Task<GameMapJsonBase?> GetMapBase(string gameNameOrId, string mapNameOrId);

    }
}
