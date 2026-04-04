namespace DomainLayer.DTOs
{
    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupervisorId { get; set; }
        public List<UserDto> Members { get; set; } = new List<UserDto>();
    }
}
