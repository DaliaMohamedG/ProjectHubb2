using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class AttachmentDTO
    {
        public string Name { get; set; } = null!;   // e.g. "design_guidelines.pdf"
        public string Type { get; set; } = null!;   // e.g. "pdf", "fig"
    }
}
