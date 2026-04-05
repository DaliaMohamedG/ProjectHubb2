using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class TaskAttachment
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;      // e.g. "design.pdf"
        public string Type { get; set; } = null!;      // e.g. "pdf", "fig"
        public string FilePath { get; set; } = null!;  // where file is stored

        // "Supervisor" or "Student" — who uploaded this file
        public string UploadedBy { get; set; } = null!;

        // Which task this belongs to
        public string TaskId { get; set; } = null!;
        [ForeignKey("TaskId")]
        public virtual TeamTasks Task { get; set; } = null!;
    }
}
