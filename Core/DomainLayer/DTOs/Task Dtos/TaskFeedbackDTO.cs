using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class TaskFeedbackDTO
    {
        public string Message { get; set; } = null!;              // feedback text
        public string From { get; set; } = null!;                 // "Supervisor" or "Assistant"
        public List<AttachmentDTO> Attachments { get; set; } = new(); // optional files      
    }
}
