namespace DomainLayer.Models
{
    public class TeamTasks
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string SolutionFile { get; set; }

        public string StudentId { get; set; }
        public User Student { get; set; }
        public string DoctorId { get; set; }
        public User Doctor { get; set; }
        public string AssistantId { get; set; }
        public User Assistant { get; set; }
    }
}
