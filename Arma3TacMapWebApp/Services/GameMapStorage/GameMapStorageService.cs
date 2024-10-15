using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arma3TacMapWebApp.Services.GameMapStorage
{
    public class GameMapStorageService : IGameMapStorageService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IMemoryCache memoryCache;
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions() { Converters = { new JsonStringEnumConverter() }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public Uri BaseUri { get; }

        public static Uri GetBaseUri(IConfiguration configuration)
            => configuration.GetValue<Uri>("GameMapStorage") ?? new Uri("https://atlas.plan-ops.fr/");

        public GameMapStorageService(IHttpClientFactory clientFactory, IMemoryCache memoryCache, IConfiguration configuration)
        {
            this.clientFactory = clientFactory;
            this.memoryCache = memoryCache;
            BaseUri = GetBaseUri(configuration);
        }

        public static void AddTo(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGameMapStorageService, GameMapStorageService>();

            services.AddHttpClient("GameMapStorage").ConfigureHttpClient(options => {
                options.BaseAddress = GetBaseUri(configuration);
            });
        }

        private async Task<T?> Get<T>(string uri, Action<T>? postProcess = null) where T : class
        {
            if (!memoryCache.TryGetValue(uri, out T? value) || value == null)
            {
                value = await GetFromApi<T>(uri);

                if (value != null)
                {
                    if (postProcess != null)
                    {
                        postProcess(value);
                    }
                    using var entry = memoryCache.CreateEntry(uri);
                    entry.SlidingExpiration = TimeSpan.FromHours(1);
                    entry.Value = value;
                }
            }
            return value;
        }

        private async Task<T?> GetFromApi<T>(string uri) where T : class
        {
            using var client = clientFactory.CreateClient("GameMapStorage");

            using var result = await client.GetAsync(uri);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<T>(jsonOptions);
            }
            
            if (result.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                result.EnsureSuccessStatusCode();
            }
            return null;
        }

        /// <summary>
        /// List supported games
        /// </summary>
        /// <returns></returns>
        public async Task<GameJsonBase[]> GetGames()
        {
            return await Get<GameJsonBase[]>("/api/v1/games") ?? [];
        }

        /// <summary>
        /// Get game metadata, standard markers, and standard colors
        /// </summary>
        /// <param name="gameNameOrId">Game name ('arma3') or GameId (1)</param>
        /// <returns></returns>
        public Task<GameJson?> GetGame(string gameNameOrId)
        {
            return Get<GameJson>($"/api/v1/games/{gameNameOrId}", EnsureContrast);
        }

        private void EnsureContrast(GameJson game)
        {
            if (game.Markers != null)
            {
                foreach (var marker in game.Markers)
                {
                    if (marker.IsColorCompatible) // Ensure contrast
                    {
                        marker.ImagePng = $"{BaseUri.AbsoluteUri}data/{game.GameId}/markers/808080/{marker.GameMarkerId}.png";
                        marker.ImageWebp = $"{BaseUri.AbsoluteUri}data/{game.GameId}/markers/808080/{marker.GameMarkerId}.webp";
                    }
                }
            }
        }

        /// <summary>
        /// List available maps of a game
        /// </summary>
        /// <param name="gameNameOrId">Game name ('arma3') or GameId (1)</param>
        /// <returns></returns>
        public async Task<GameMapJsonBase[]> GetMaps(string gameNameOrId)
        {
            return await Get<GameMapJsonBase[]>($"/api/v1/games/{gameNameOrId}/maps") ?? [];
        }

        /// <summary>
        /// Get map metadata, available layers, and locations
        /// </summary>
        /// <param name="gameNameOrId">Game name ('arma3') or GameId (1)</param>
        /// <param name="mapNameOrId">Map name ('altis') or GameMapId (1)</param>
        /// <returns></returns>
        public Task<GameMapJson?> GetMap(string gameNameOrId, string mapNameOrId)
        {
            return Get<GameMapJson>($"/api/v1/games/{gameNameOrId}/maps/{mapNameOrId}");
        }

        /// <summary>
        /// List paper maps of a map
        /// </summary>
        /// <param name="gameNameOrId">Game name ('arma3') or GameId (1)</param>
        /// <param name="mapNameOrId">Map name ('altis') or GameMapId (1)</param>
        /// <returns></returns>
        public async Task<GamePaperMapJson[]> GetMapPaperMaps(string gameNameOrId, string mapNameOrId)
        {
            return await Get<GamePaperMapJson[]>($"/api/v1/games/{gameNameOrId}/maps/{mapNameOrId}/papermaps") ?? [];
        }

        /// <summary>
        /// List paper maps of all maps of a game
        /// </summary>
        /// <param name="gameNameOrId">Game name ('arma3') or GameId (1)</param>
        /// <returns></returns>
        public async Task<GamePaperMapMapJson[]> GetMapPaperMaps(string gameNameOrId)
        {
            return await Get<GamePaperMapMapJson[]>($"/api/v1/games/{gameNameOrId}/papermaps") ?? [];
        }

        public async Task<GameJsonBase?> GetGameBase(string gameNameOrId)
        {
            return (await GetGames().ConfigureAwait(false)).FirstOrDefault(g => g.Name == gameNameOrId);
        }

        public async Task<GameMapJsonBase?> GetMapBase(string gameNameOrId, string mapNameOrId)
        {
            return (await GetMaps(gameNameOrId).ConfigureAwait(false)).FirstOrDefault(g => g.Name == mapNameOrId);
        }
    }
}
