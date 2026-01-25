namespace DomainLayer.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string SolutionFile { get; set; }


        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }
        public int AssistantId { get; set; }
        public Assistant Assistant { get; set; }
    }
}
