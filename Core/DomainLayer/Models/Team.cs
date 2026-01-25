namespace DomainLayer.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Meeting> Meetings { get; set; }
    }
}
