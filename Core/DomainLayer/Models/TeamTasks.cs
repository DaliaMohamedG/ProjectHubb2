using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class TeamTasks
    {
        public string Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool Status { get; set; } = false;
        public string? SolutionFile { get; set; }

        public int TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        public virtual ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();

        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }

}
