using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class CreateTaskDTO
    {
        public string Details { get; set; } = null!;        
        public DateTime Deadline { get; set; }               
        public int TeamId { get; set; }                      
        public string? AssignedStudentId { get; set; }       
    }
}
