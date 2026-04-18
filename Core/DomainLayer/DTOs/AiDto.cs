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
}
