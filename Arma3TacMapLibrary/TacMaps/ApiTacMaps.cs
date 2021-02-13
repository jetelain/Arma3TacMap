using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Arma3TacMapLibrary.TacMaps
{
    public class ApiTacMaps : IApiTacMaps
    {
        private readonly HttpClient _httpClient;

        public ApiTacMaps (HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ApiTacMap>> List()
        {
            var result = await _httpClient.GetAsync($"/api/TacMaps");
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<List<ApiTacMap>>();
        }

        public async Task<ApiTacMap> Get(int id)
        {
            var result = await _httpClient.GetAsync($"/api/TacMaps/{id}");
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<ApiTacMap>();
        }

        public async Task<ApiTacMap> Create(ApiTacMapCreate tacMap)
        {
            var result = await _httpClient.PostAsync("/api/TacMaps", JsonContent.Create(tacMap));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<ApiTacMap>();
        }

        public async Task<ApiTacMap> Update(int id, ApiTacMapPatch tacMap)
        {
            var result = await _httpClient.PatchAsync($"/api/TacMaps/{id}", JsonContent.Create(tacMap));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<ApiTacMap>();
        }

        public async Task Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"/api/TacMaps/{id}");
            result.EnsureSuccessStatusCode();
        }
    }
}
