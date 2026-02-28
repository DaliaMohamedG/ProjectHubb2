namespace DomainLayer.DTOs
{
    public class UserSyncDto
    {
        public string Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Instituation { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public string Track { get; set; }
        public string UserType { get; set; }
    }
}
