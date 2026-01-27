namespace DomainLayer.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public PostStatus Status { get; set; } 

        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        public int CommunityId { get; set; }
        public Community Community { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}
