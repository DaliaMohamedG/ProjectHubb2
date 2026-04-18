using DomainLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;

namespace Graduation_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;
        public AiController(IAiService aiService) => _aiService = aiService;

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] string idea)
            => Ok(await _aiService.AnalyzeIdeaAsync(idea));

        [HttpPost("suggest")]
        public async Task<IActionResult> Suggest([FromBody] SuggestRequest dto)
            => Ok(await _aiService.SuggestProjectsAsync(dto));

        [HttpGet("meta")]
        public async Task<ActionResult<AiMetaDto>> GetMeta()
        {
            var meta = await _aiService.GetAiMetadataAsync();

            if (meta == null) return NotFound("Could not fetch AI metadata.");

            return Ok(meta);
        }
    }
}
