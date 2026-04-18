using DomainLayer.DTOs;

namespace ServicesAbstractionLayer
{
    public interface IAiService
    {
        Task<string> AnalyzeIdeaAsync(string idea);
        Task<string> SuggestProjectsAsync(SuggestRequest dto);
        Task<AiMetaDto> GetAiMetadataAsync();

    }
}
