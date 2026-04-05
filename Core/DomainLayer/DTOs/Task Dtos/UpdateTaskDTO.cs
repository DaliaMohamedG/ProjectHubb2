using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class UpdateTaskDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string>? AssignedTo { get; set; }
    }
}
