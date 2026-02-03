using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Meeting> Meetings { get; set; }
    }
}
