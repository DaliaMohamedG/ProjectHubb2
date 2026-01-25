namespace DomainLayer.Models
{
    public class Admin : User
    {
        public ICollection<Post> Posts { get; set; }
    }
}
