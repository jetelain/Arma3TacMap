using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Arma3TacMapWebApp.Maps
{
    public class MapInfosService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _cache;

        public MapInfosService(IMemoryCache cache, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _cache = cache;
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

        private async Task<List<MapInfos>> ReadMapsInfos()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://jetelain.github.io/Arma3Map/maps/all.json");
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
                        }
                        return data.Values.OrderBy(v => v.worldName).ToList();
                    }
                }
            }
            catch
            {

            }
            return new List<MapInfos>();
        }
    }
}
