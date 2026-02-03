using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class TeamTasks
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public DateTime Deadline { get; set; }

        [StringLength(500)]
        public string Status { get; set; }
        public string SolutionFile { get; set; }

        public string? AssignedStudentId { get; set; }
        [ForeignKey("AssignedStudentId")]
        public virtual Student? Student { get; set; }

        public string? AssignedDoctorId { get; set; }
        [ForeignKey("AssignedDoctorId")]
        public virtual Doctor? Doctor { get; set; }

        public string? AssignedAssistantId { get; set; }
        [ForeignKey("AssignedAssistantId")]
        public virtual Assistant? Assistant { get; set; }
    }
}
