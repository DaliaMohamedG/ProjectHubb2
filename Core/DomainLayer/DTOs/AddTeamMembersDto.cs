namespace DomainLayer.DTOs
{
    public class AddTeamMembersDto
    {
        public int TeamId { get; set; }
        public List<string> MemberIds { get; set; }
    }
}
