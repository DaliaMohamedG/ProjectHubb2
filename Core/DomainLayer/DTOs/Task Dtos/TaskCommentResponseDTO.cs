using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class TaskCommentResponseDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;       
        public string? UserImage { get; set; }               
        public DateTime CreatedAt { get; set; }
    }
}
