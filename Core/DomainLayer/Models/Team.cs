using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }

        public string SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public virtual Supervisor Supervisor { get; set; }

        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<TeamTasks> Tasks { get; set; } = new List<TeamTasks>();
    }
}
