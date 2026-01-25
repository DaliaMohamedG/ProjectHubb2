namespace DomainLayer.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Message> Messages { get; set; }

    }
}
