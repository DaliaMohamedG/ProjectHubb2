namespace DomainLayer.Models
{
    public class Admin
    {
        public string Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public ICollection<Post> Posts { get; set; }
    }
}
