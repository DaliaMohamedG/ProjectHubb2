namespace DomainLayer.Models
{
    public class Student : User
    {
        public string GraduationProjectDatails { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Skills { get; set; } = null!;
        public string University_Name { get; set; } = null!;

        public ICollection<Team> Teams { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
