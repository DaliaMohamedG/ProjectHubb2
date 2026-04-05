using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class FeedbackResponseDTO
    {
        public string Id { get; set; } = null!;
        public string TaskId { get; set; } = null!;
        public string From { get; set; } = null!;              // "Supervisor" or "Assistant"
        public string Message { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<AttachmentDTO> Attachments { get; set; } = new();
    }
}
