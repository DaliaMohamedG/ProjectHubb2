using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class CompletedTaskDTO
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public DateTime CompletedDate { get; set; }
        public string Status { get; set; } = null!;   // "Approved" or "NeedsRevision"
        public bool HasFeedback { get; set; } = true;
    }
}
