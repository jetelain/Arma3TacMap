using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
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

        public async Task<ApiTacMap> Get(string link)
        {
            var uri = new Uri(link, UriKind.Absolute);
            if (uri.DnsSafeHost != _httpClient.BaseAddress.DnsSafeHost)
            {
                _logger.LogWarning("Hostname mismatch, '{0}' != '{1}'", uri.DnsSafeHost, _httpClient.BaseAddress.DnsSafeHost);
                return null;
            }

            var readOnly  = uri.AbsolutePath.StartsWith("/ViewMap/");
            var readWrite = uri.AbsolutePath.StartsWith("/EditMap/");
            if (!readOnly && !readWrite)
            {
                _logger.LogWarning("Unknown path '{0}'", uri.AbsolutePath);
                return null;
            }

            int id;
            if(!int.TryParse(uri.AbsolutePath.Substring(9), out id))
            {
                _logger.LogWarning("Unknown id '{0}'", uri.AbsolutePath);
                return null;
            }

            var token = HttpUtility.ParseQueryString(uri.Query).Get("t");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Missing token '{0}'", uri.Query);
                return null;
            }
            var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
                { readOnly ? "readOnlyToken" : "readWriteToken",  token}
            });
            var result = await _httpClient.PostAsync($"/api/TacMaps/{id}/grant", content);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<ApiTacMap>();
        }

        public bool IsTacMapLink(string link)
        {
            var uri = new Uri(link, UriKind.Absolute);
            if (uri.DnsSafeHost != _httpClient.BaseAddress.DnsSafeHost)
            {
                return false;
            }
            var readOnly = uri.AbsolutePath.StartsWith("/ViewMap/");
            var readWrite = uri.AbsolutePath.StartsWith("/EditMap/");
            if (!readOnly && !readWrite)
            {
                return false;
            }
            int id;
            if (!int.TryParse(uri.AbsolutePath.Substring(9), out id))
            {
                return false;
            }
            var token = HttpUtility.ParseQueryString(uri.Query).Get("t");
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            return true;
        }
    }
}
