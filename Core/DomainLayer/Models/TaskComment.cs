using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public string TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual TeamTasks Task { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
