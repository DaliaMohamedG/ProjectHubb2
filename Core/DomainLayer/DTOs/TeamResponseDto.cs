namespace DomainLayer.DTOs
{
    public class TeamResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public string SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public int ActiveProjects { get; set; }
        public List<TeamMemberResponseDto> Assistants { get; set; } = new();
        public List<TeamMemberResponseDto> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
