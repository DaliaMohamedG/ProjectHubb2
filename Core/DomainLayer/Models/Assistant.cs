namespace DomainLayer.Models
{
    public class Assistant : User
    {
        public string University_Name { get; set; } = null!;

        public ICollection<Project> Projects { get; set; }
        public ICollection<TeamTasks> Tasks { get; set; }
    }
}
