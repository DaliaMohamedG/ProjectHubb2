using System.Net.Http.Json;
using DomainLayer.DTOs;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://aya3330-projhub-ai.hf.space";

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AnalyzeIdeaAsync(string idea)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/analyze", new { idea = idea });
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SuggestProjectsAsync(SuggestRequest dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/suggest", dto);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<AiMetaDto> GetAiMetadataAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/meta");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AiMetaDto>();
            }
            return new AiMetaDto();
        }
    }
}
