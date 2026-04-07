namespace DomainLayer.DTOs
{
    public class TeamMemberResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Role { get; set; }
        public string? Position { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
