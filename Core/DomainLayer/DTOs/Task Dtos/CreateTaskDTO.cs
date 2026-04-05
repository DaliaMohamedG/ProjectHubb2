using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class CreateTaskDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }         
        public DateTime DueDate { get; set; }               
        public int TeamId { get; set; }
        public List<string> AssignedTo { get; set; } = new();
    }
}
