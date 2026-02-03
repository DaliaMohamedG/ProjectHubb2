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

        public Team Team { get; set; }
        public string? AssignedDoctorId { get; set; }
        [ForeignKey("AssignedDoctorId")]
        public virtual Doctor? Doctor { get; set; }

        public string? AssignedAssistantId { get; set; }
        [ForeignKey("AssignedAssistantId")]
        public virtual Assistant? Assistant { get; set; }
        public ICollection<Sponsor> Sponsors { get; set; }
        public ICollection<Conversation> Conversations { get; set; }
    }
}
