using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class SubmitTaskDTO
    {
        public List<AttachmentDTO> StudentAttachments { get; set; } = new();
    }
}
