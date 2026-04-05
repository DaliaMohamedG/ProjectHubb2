using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.PostDtos
{
    public class PostUpdateDto
    {
        public string? Content { get; set; }
        public string? AttachmentName { get; set; }
        public string? Visibility { get; set; }
    }
}
