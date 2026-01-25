namespace DomainLayer.Models
{
    public class Doctor : User
    {
        public string Specialization { get; set; } = null!;
        public string University_Name { get; set; } = null!;

        public ICollection<Project> Projects { get; set; }
        public ICollection<TeamTasks> Tasks { get; set; }
        public ICollection<Meeting> Meetings { get; set; }
        public ICollection<Team> Teams { get; set; }
    }
}
