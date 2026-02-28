namespace DomainLayer.Models
{
    public class Supervisor : User
    {
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<TeamTasks> Tasks { get; set; } = new List<TeamTasks>();
    }
}
