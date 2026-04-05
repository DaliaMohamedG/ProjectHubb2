using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class TaskAssignment
    {
        public string Id { get; set; }

        // Which task
        public string TaskId { get; set; } = null!;
        [ForeignKey("TaskId")]
        public virtual TeamTasks Task { get; set; } = null!;

        // Which student
        public string StudentId { get; set; } = null!;
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
    }
}
