namespace DomainLayer.Models
{
    public class TeamMember
    {
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string RoleInTeam { get; set; } 
    }
}
