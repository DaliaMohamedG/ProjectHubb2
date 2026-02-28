using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace PersistenceLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<TeamTasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. إعدادات الوراثة (Inheritance) - التخزين في جداول منفصلة
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Supervisor>().ToTable("Supervisors");
            modelBuilder.Entity<Assistant>().ToTable("Assistants");

            // 2.  (Conversation)
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

                entity.HasOne(c => c.Project)
                      .WithMany()
                      .HasForeignKey(c => c.ProjectId)
                      .OnDelete(DeleteBehavior.SetNull); // لو المشروع اتحذف المحادثة تفضل
            });

            // 3.  (Message)
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Conversation)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(m => m.Conversation_ID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.User)
                      .WithMany(u => u.Messages)
                      .HasForeignKey(m => m.UserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 4.  (TeamTasks)
            modelBuilder.Entity<TeamTasks>(entity =>
            {
                entity.HasOne(t => t.Team)
                      .WithMany(tm => tm.Tasks)
                      .HasForeignKey(t => t.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Student)
                      .WithMany(s => s.Tasks)
                      .HasForeignKey(t => t.AssignedStudentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 5. (Post)
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Community)
                      .WithMany()
                      .HasForeignKey(p => p.CommunityId)
                      .OnDelete(DeleteBehavior.Cascade);

                // ميزة الخصوصية :البوست تابع لتيم معين
                entity.HasOne(p => p.Team)
                      .WithMany()
                      .HasForeignKey(p => p.TeamId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // 6
            modelBuilder.Entity<Team>(entity =>
                entity.HasOne(t => t.Supervisor)
                      .WithMany()
                      .HasForeignKey(t => t.SupervisorId)
                      .OnDelete(DeleteBehavior.Restrict));

            // 7
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Students)
                .WithMany(s => s.Teams)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentTeam",
                    j => j.HasOne<Student>().WithMany().HasForeignKey("StudentsId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<Team>().WithMany().HasForeignKey("TeamsId").OnDelete(DeleteBehavior.NoAction)
                );
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasOne(t => t.Supervisor)
                      .WithMany()
                      .HasForeignKey(t => t.SupervisorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 8
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Supervisor)
                      .WithMany(s => s.Projects)
                      .HasForeignKey(p => p.AssignedSupervisorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Team)
                      .WithOne()
                      .HasForeignKey<Project>(p => p.TeamId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Supervisor)
                      .WithMany(s => s.Projects)
                      .HasForeignKey(p => p.AssignedSupervisorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}