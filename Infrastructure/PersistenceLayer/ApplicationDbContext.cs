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
        public DbSet<TeamTasks> Tasks { get; set; } // لاحظي هنا الاسم في DB هيكون Tasks
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

            modelBuilder.Entity<User>().Property(u => u.Id).HasMaxLength(200);

            // TPH (Table Per Hierarchy) Configuration
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserRole")
                .HasValue<Student>("Student")
                .HasValue<Doctor>("Doctor")
                .HasValue<Assistant>("Assistant")
                .HasValue<Admin>("Admin");

            // 1 to 1: Team and Project
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Project)
                .WithOne(p => p.Team)
                .HasForeignKey<Team>(t => t.ProjectId);

            // Composite Keys (Weak Entities)
            modelBuilder.Entity<Comment>()
                .HasKey(c => new { c.Id, c.PostId });

            modelBuilder.Entity<Message>()
                .HasKey(m => new { m.Id, m.Conversation_ID });

            // Unique Constraints
            modelBuilder.Entity<Meeting>().HasIndex(m => m.ZoomLink).IsUnique();
            modelBuilder.Entity<Team>().HasIndex(t => t.TeamName).IsUnique();

            // Post & User Relationship (لحل مشكلة الـ Shadow Property في الـ Post)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project Relationships
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Assistant)
                .WithMany()
                .HasForeignKey(p => p.AssistantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Conversation Relationships
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.User_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.TargetUser)
                .WithMany()
                .HasForeignKey(c => c.TargetUser_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // TeamTasks (Tasks Table) Relationships
            modelBuilder.Entity<TeamTasks>(entity =>
            {
                entity.HasOne(t => t.Assistant)
                    .WithMany()
                    .HasForeignKey(t => t.AssistantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Student)
                    .WithMany()
                    .HasForeignKey(t => t.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Doctor)
                    .WithMany()
                    .HasForeignKey(t => t.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // حل مشكلة المقابلات (Meeting)
            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.NoAction); // ده السطر السحري

            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.Team)
                .WithMany()
                .HasForeignKey(m => m.TeamId)
                .OnDelete(DeleteBehavior.NoAction); // وده كمان
                                                    // حل مشكلة علاقة الطلاب بالفرق (Many-to-Many)
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Students)
                .WithMany(s => s.Teams)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentTeam",
                    j => j.HasOne<Student>().WithMany().HasForeignKey("StudentsId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<Team>().WithMany().HasForeignKey("TeamsId").OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}