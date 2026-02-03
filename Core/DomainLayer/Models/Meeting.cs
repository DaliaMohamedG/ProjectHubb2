using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Meeting
    {
        public int Id { get; set; }
        public DateTime ScheduleTime { get; set; }
        [StringLength(500)]
        public string ZoomLink { get; set; } // unique
        public string Details { get; set; }
        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
