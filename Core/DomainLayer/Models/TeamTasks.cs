using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class TeamTasks
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } // "Pending", "Done"
        public string? SolutionFile { get; set; }

        public int TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        public string? AssignedStudentId { get; set; }
        [ForeignKey("AssignedStudentId")]
        public virtual Student? Student { get; set; }

        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }

}
