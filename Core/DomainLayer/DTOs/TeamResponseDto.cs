namespace DomainLayer.DTOs
{
    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public int ActiveProjects { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
        public List<TeamMemberDto> Assistants { get; set; } = new List<TeamMemberDto>();
    }
}
