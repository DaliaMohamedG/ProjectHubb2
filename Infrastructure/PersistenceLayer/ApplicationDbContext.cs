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
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<TeamTasks> Tasks { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>().ToTable("Admins");

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Doctor>().ToTable("Doctors");
            modelBuilder.Entity<Assistant>().ToTable("Assistants");

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasOne(c => c.Sender)
                      .WithMany()
                      .HasForeignKey(c => c.Sender_ID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.TargetUser)
                      .WithMany()
                      .HasForeignKey(c => c.TargetUser_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Conversation)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(m => m.Conversation_ID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                      .WithMany()
                      .HasForeignKey(m => m.Sender_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.HasOne(m => m.Doctor)
                      .WithMany(d => d.Meetings)
                      .HasForeignKey(m => m.DoctorId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(m => m.Team)
                      .WithMany(t => t.Meetings)
                      .HasForeignKey(m => m.TeamId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(m => m.ZoomLink).IsUnique();
            });

            modelBuilder.Entity<TeamTasks>(entity =>
            {
                entity.HasOne(t => t.Student)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedStudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Doctor)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedDoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Assistant)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedAssistantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Community)
                      .WithMany(c => c.Posts)
                      .HasForeignKey(p => p.CommunityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Project)
                .WithOne(p => p.Team)
                .HasForeignKey<Team>(t => t.ProjectId);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Students)
                .WithMany(s => s.Teams)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentTeam",
                    j => j.HasOne<Student>().WithMany().HasForeignKey("StudentsId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<Team>().WithMany().HasForeignKey("TeamsId").OnDelete(DeleteBehavior.NoAction)
                );

            modelBuilder.Entity<User>().Property(u => u.Id).HasMaxLength(255);
            modelBuilder.Entity<Admin>().Property(a => a.Id).HasMaxLength(255);

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Doctor)
                 .WithMany()
                 .HasForeignKey(p => p.AssignedDoctorId)
                 .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Assistant)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedAssistantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}