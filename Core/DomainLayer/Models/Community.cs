namespace DomainLayer.Models
{
    public class Community
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
