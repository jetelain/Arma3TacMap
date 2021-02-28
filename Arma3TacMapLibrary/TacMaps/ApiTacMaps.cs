using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Arma3TacMapLibrary.TacMaps
{
    public class ApiTacMaps : IApiTacMaps
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ApiTacMaps> _logger;

        public ApiTacMaps (HttpClient httpClient, IMemoryCache cache, ILogger<ApiTacMaps> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<ApiTacMap>> List()
        {
            var result = await _httpClient.GetAsync($"/api/TacMaps");
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<List<ApiTacMap>>();
        }

        public async Task<ApiTacMap> Get(int id)
        {
            var key = $"/api/TacMaps/{id}";
            ApiTacMap value;
            if (_cache.TryGetValue(key, out value))
            {
                return value;
            }
            try
            {
                var result = await _httpClient.GetAsync($"/api/TacMaps/{id}");
                result.EnsureSuccessStatusCode();
                value = await result.Content.ReadFromJsonAsync<ApiTacMap>();
                _cache.Set(key, value, TimeSpan.FromHours(1));
                return value;
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, "Failed");
                return null;
            }
        }

        public async Task<ApiTacMap> Create(ApiTacMapCreate tacMap)
        {
            var result = await _httpClient.PostAsync("/api/TacMaps", JsonContent.Create(tacMap));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<ApiTacMap>();
        }

        public async Task<ApiTacMap> Update(int id, ApiTacMapPatch tacMap)
        {
            _cache.Remove($"/api/TacMaps/{id}");
            var result = await _httpClient.PatchAsync($"/api/TacMaps/{id}", JsonContent.Create(tacMap));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<ApiTacMap>();
        }

        public async Task Delete(int id)
        {
            _cache.Remove($"/api/TacMaps/{id}");
            var result = await _httpClient.DeleteAsync($"/api/TacMaps/{id}");
            result.EnsureSuccessStatusCode();
        }
    }
}
