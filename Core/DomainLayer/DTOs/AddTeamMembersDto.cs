namespace DomainLayer.DTOs
{
    public class AddTeamMembersDto
    {
        public string TeamId { get; set; }
        public List<string> MemberIds { get; set; }
    }
}
