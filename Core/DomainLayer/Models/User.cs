namespace DomainLayer.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Profile_Image { get; set; } = null!;

        public ICollection<Post> Posts { get; set; }
        public ICollection<Conversation> Conversations { get; set; }
    }
}
