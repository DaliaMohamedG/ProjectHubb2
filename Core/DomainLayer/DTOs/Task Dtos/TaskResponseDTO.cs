using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class TaskResponseDTO
    {
        public int Id { get; set; }
        public string Details { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = null!;
        public string? SolutionFile { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public string? AssignedStudentId { get; set; }
        public string? AssignedStudentName { get; set; }
        public List<TaskCommentResponseDTO> Comments { get; set; } = new();
        public bool IsOverdue => Status != "Done" && DateTime.UtcNow > Deadline;
    }
}
