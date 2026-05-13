using DomainLayer.Contracts;
using Microsoft.EntityFrameworkCore;
using PeresentationLayer.Hubs;
using PersistenceLayer;
using PersistenceLayer.Repositories;
using ServicesAbstractionLayer;
using ServicesLayer;

namespace Graduation_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Notification
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddScoped<NotificationService>();
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<ICommunityService, CommunityService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<ServicesAbstractionLayer.IAuthService, ServicesLayer.AuthService>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddHttpClient<IAiService, AiService>();

            #region Notification
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyOrigin()   // ?? ??? domain ??????? ?? ??????
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    // ??????: ?? AllowAnyOrigin ?? ?????? ???? AllowCredentials
                    // ?? Flutter ?? ?????? credentials ??? ????
                });
            });
            #endregion

            var app = builder.Build();

            #region Notification
            app.UseCors("CorsPolicy");  // ? ???? ???? ??? UseAuthorization
            #endregion

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            #region Notification
            app.MapHub<NotificationHub>("/notificationHub");
            #endregion

            app.Run();
        }
    }
}