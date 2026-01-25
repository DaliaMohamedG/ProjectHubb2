namespace DomainLayer.Models
{
    public class Sponsor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string ContactInfo { get; set; }
        public string Logo { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
