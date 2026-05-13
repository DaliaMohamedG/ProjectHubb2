namespace DomainLayer.DTOs
{
    public class SuggestRequest
    {
        public List<SkillDto> Skills { get; set; }
        public string Domain { get; set; }
    }

    public class SkillDto
    {
        public string Track { get; set; }
        public string Level { get; set; }
    }

    public class AnalyzeRequest
    {
        public string Idea { get; set; }
    }
    public class AiMetaDto
    {
        public List<string> Tracks { get; set; }
        public List<string> Levels { get; set; }
        public List<string> Domains { get; set; }
    }
    // What her Python system sends to your endpoint
    public class AiRecommendationResultDto
    {
        public string Scenario { get; set; }        // "suggest" or "analyze"
        public List<AiProjectIdeaDto> Ideas { get; set; } = new();
    }

    public class AiProjectIdeaDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tracks { get; set; } = new();
        public AiTechStackDto TechStack { get; set; }
        public List<string> HowItWorks { get; set; } = new();   // the steps
        public List<AiSimilarProjectDto> SimilarProjects { get; set; } = new();
    }

    public class AiTechStackDto
    {
        public string Frontend { get; set; }
        public string Backend { get; set; }
        public string Database { get; set; }
        public string Hosting { get; set; }
    }

    public class AiSimilarProjectDto
    {
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public int SimilarityPercent { get; set; }   // e.g. 30
        public string Tags { get; set; }
        public string GithubUrl { get; set; }
    }
}