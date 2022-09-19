using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Arma3TacMapLibrary.Arma3
{
    public class MapInfosService : IMapInfosService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<MapInfosService> _logger;
        private readonly IMemoryCache _cache;
        private readonly string _endpoint;

        public MapInfosService(IMemoryCache cache, IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<MapInfosService> logger)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _cache = cache;
            _endpoint = Arma3MapHelper.GetEndpoint(configuration);
        }

        public async Task<List<MapInfos>> GetMapsInfos()
        {
            List<MapInfos> value;
            if (_cache.TryGetValue(nameof(MapInfos), out value))
            {
                return value;
            }
            value = await ReadMapsInfos();
            _cache.Set(nameof(MapInfos), value, TimeSpan.FromMinutes(60));
            return value;
        }

        public async Task<MapInfos> GetMapsInfos(string worldName)
        {
            return (await GetMapsInfos()).FirstOrDefault(m => string.Equals(m.worldName, worldName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<List<MapInfos>> ReadMapsInfos()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + "/maps/all.json");
                request.Headers.Add("User-Agent","Mozilla/5.0 (X11; Linux x86_64; rv:60.0) Gecko/20100101 Firefox/81.0");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, MapInfos>>(responseStream, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        foreach(var pair in data)
                        {
                            pair.Value.worldName = pair.Key;
                            pair.Value.tilePattern = _endpoint + pair.Value.tilePattern;
                            pair.Value.fullMapTile = pair.Value.tilePattern.Replace("{x}", "0").Replace("{y}", "0").Replace("{z}", "0");
                        }
                        return data.Values.OrderBy(v => v.worldName).ToList();
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, "Failed");
            }
            return new List<MapInfos>();
        }
    }
}
