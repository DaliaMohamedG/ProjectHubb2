using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace PersistenceLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TeamTasks> Tasks { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserRole")
                .HasValue<Student>("Student")
                .HasValue<Doctor>("Doctor")
                .HasValue<Assistant>("Assistant")
                .HasValue<Admin>("Admin");

            //(One-to-One)
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Project)
                .WithOne(p => p.Team)
                .HasForeignKey<Team>(t => t.ProjectId);

            // Weak Entities
            modelBuilder.Entity<Comment>()
                .HasKey(c => new { c.Id, c.PostId });
            modelBuilder.Entity<Message>()
                .HasKey(m => new { m.Id, m.ConversationId });

            //  (Unique Constraints)
            modelBuilder.Entity<Meeting>()
                .HasIndex(m => m.ZoomLink)
                .IsUnique();

            modelBuilder.Entity<Team>()
                .HasIndex(t => t.TeamName)
                .IsUnique();

            // 5. علاقة Many-to-Many بين Project و Sponsor
            // في EF Core 5+ ممكن تتعمل تلقائي، لكن يفضل تعريفها لو محتاجة تحكم
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Sponsors)
                .WithMany(s => s.Projects)
                .UsingEntity(j => j.ToTable("ProjectSponsors"));

            // 6. تحديد الـ Cascade Delete
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
