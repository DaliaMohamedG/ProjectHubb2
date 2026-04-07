namespace DomainLayer.Models
{
    public class Like
    {
        public string PostId { get; set; }
        public virtual Post Post { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
