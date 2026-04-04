namespace DomainLayer.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? SupervisorId { get; set; }
        public virtual User? Supervisor { get; set; }
        public virtual ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public virtual ICollection<TeamTasks> Tasks { get; set; } = new List<TeamTasks>();
    }
}