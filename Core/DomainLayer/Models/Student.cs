namespace DomainLayer.Models
{
    public class Student : User
    {
        public string Track { get; set; } = null!;

        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public virtual ICollection<TeamTasks> Tasks { get; set; } = new List<TeamTasks>();
    }
}
