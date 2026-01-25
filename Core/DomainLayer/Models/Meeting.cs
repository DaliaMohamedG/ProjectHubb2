namespace DomainLayer.Models
{
    public class Meeting
    {
        public int Id { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string ZoomLink { get; set; } // unique
        public string Details { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
