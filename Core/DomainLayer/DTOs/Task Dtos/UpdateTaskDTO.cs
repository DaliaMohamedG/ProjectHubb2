using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.Task_Dtos
{
    public class UpdateTaskDTO
    {
        public string? Details { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AssignedStudentId { get; set; }
    }
}
