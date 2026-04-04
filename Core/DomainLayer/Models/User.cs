namespace DomainLayer.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Instituation { get; set; } = null!;
        public string? Faculty { get; set; }
        public string? Profile_Image { get; set; }
        public string? Role { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
