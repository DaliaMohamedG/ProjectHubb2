using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TechnologyUsed { get; set; }
        public string ProjectFilePath { get; set; }
        public string GithubUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        public string? AssignedSupervisorId { get; set; }
        [ForeignKey("AssignedSupervisorId")]
        public virtual Supervisor? Supervisor { get; set; }

        public string? AssignedAssistantId { get; set; }
        [ForeignKey("AssignedAssistantId")]
        public virtual Assistant? Assistant { get; set; }

        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; }

    }
}
