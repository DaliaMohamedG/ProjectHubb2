namespace DomainLayer.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public DateTime Start_Date { get; set; }
        public string User_ID { get; set; }
        public User User { get; set; }
        public string TargetUser_ID { get; set; }
        public User TargetUser { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
