using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class TaskResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string From { get; set; } = null!;              // who assigned: "Supervisor"
        public DateTime DueDate { get; set; }
        public string TeamId { get; set; } = null!;
        public string SupervisorId { get; set; } = null!;
        public List<string> AssignedTo { get; set; } = new(); // list of student IDs
        public bool IsCompleted { get; set; }                  // true = Done, false = Pending
        public List<AttachmentDTO> SupervisorAttachments { get; set; } = new();
        public List<AttachmentDTO> StudentAttachments { get; set; } = new();
    }
}
