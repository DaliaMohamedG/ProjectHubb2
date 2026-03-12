using DomainLayer.Contracts;
using DomainLayer.DTOs.Task_Dtos;
using DomainLayer.Models;
using ServicesAbstractionLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ─────────────────────────────────────────────────────
        // CREATE TASK
        // Only supervisor of the team can create a task
        // ─────────────────────────────────────────────────────
        public async Task<TaskResponseDTO> CreateTaskAsync(string supervisorId, CreateTaskDTO dto)
        {
            // Step 1: Check the team exists AND belongs to this supervisor
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == dto.TeamId && t.SupervisorId == supervisorId);

            if (!teams.Any())
                throw new UnauthorizedAccessException("You can only assign tasks to your own teams.");

            // Step 2: If assigning to specific student, make sure they are in the team
            if (dto.AssignedStudentId != null)
            {
                var team = teams.First();
                bool studentInTeam = team.Students.Any(s => s.Id == dto.AssignedStudentId);
                if (!studentInTeam)
                    throw new InvalidOperationException("This student is not a member of this team.");
            }

            // Step 3: Create the task
            var task = new TeamTasks
            {
                Details = dto.Details,
                Deadline = dto.Deadline,
                Status = "Pending",
                TeamId = dto.TeamId,
                AssignedStudentId = dto.AssignedStudentId
            };

            // Step 4: Save to database using CompleteAsync() ✅
            await _unitOfWork.Repository<TeamTasks>().AddAsync(task);
            await _unitOfWork.CompleteAsync();

            return MapToTaskResponse(task, teams.First());
        }

        // ─────────────────────────────────────────────────────
        // UPDATE TASK
        // ─────────────────────────────────────────────────────
        public async Task<TaskResponseDTO?> UpdateTaskAsync(int taskId, string supervisorId, UpdateTaskDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return null;

            // Check this supervisor owns the team
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId && t.SupervisorId == supervisorId);
            if (!teams.Any()) return null;

            // Only update fields that were actually sent
            if (dto.Details != null) task.Details = dto.Details;
            if (dto.Deadline.HasValue) task.Deadline = dto.Deadline.Value;
            if (dto.AssignedStudentId != null) task.AssignedStudentId = dto.AssignedStudentId;

            _unitOfWork.Repository<TeamTasks>().Update(task);
            await _unitOfWork.CompleteAsync();

            return MapToTaskResponse(task, teams.First());
        }

        // ─────────────────────────────────────────────────────
        // DELETE TASK
        // ─────────────────────────────────────────────────────
        public async Task<bool> DeleteTaskAsync(int taskId, string supervisorId)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return false;

            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId && t.SupervisorId == supervisorId);
            if (!teams.Any()) return false;

            _unitOfWork.Repository<TeamTasks>().Delete(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────
        // SUBMIT TASK
        // Student uploads their solution file
        // ─────────────────────────────────────────────────────
        public async Task<bool> SubmitTaskAsync(int taskId, string studentId, SubmitTaskDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return false;

            // If assigned to specific student → only that student can submit
            if (task.AssignedStudentId != null && task.AssignedStudentId != studentId)
                return false;

            // Make sure student is in the team
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId);
            var team = teams.FirstOrDefault();
            if (team == null || !team.Students.Any(s => s.Id == studentId))
                return false;

            task.SolutionFile = dto.SolutionFile;
            task.Status = "Done";

            _unitOfWork.Repository<TeamTasks>().Update(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────
        // GIVE FEEDBACK
        // Supervisor updates task status after reviewing
        // ─────────────────────────────────────────────────────
        public async Task<bool> GiveFeedbackAsync(int taskId, string supervisorId, TaskFeedbackDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return false;

            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId && t.SupervisorId == supervisorId);
            if (!teams.Any()) return false;

            task.Status = dto.Status;
            _unitOfWork.Repository<TeamTasks>().Update(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────
        // GET TASK BY ID (full details with comments)
        // ─────────────────────────────────────────────────────
        public async Task<TaskResponseDTO?> GetTaskByIdAsync(int taskId)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return null;

            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId);

            return MapToTaskResponse(task, teams.FirstOrDefault());
        }

        // ─────────────────────────────────────────────────────
        // GET TASKS BY TEAM (summary list)
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TaskSummaryDTO>> GetTasksByTeamAsync(int teamId)
        {
            var tasks = await _unitOfWork.Repository<TeamTasks>()
                .FindAsync(t => t.TeamId == teamId);
            return tasks.Select(MapToTaskSummary);
        }

        // ─────────────────────────────────────────────────────
        // GET TASKS BY STUDENT (My Work screen)
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TaskSummaryDTO>> GetTasksByStudentAsync(string studentId)
        {
            var tasks = await _unitOfWork.Repository<TeamTasks>()
                .FindAsync(t => t.AssignedStudentId == studentId);
            return tasks.Select(MapToTaskSummary);
        }

        // ─────────────────────────────────────────────────────
        // GET TASKS BY SUPERVISOR (All Tasks screen)
        // ─────────────────────────────────────────────────────
        public async Task<IEnumerable<TaskSummaryDTO>> GetTasksBySupervisorAsync(string supervisorId)
        {
            // First get all team IDs that belong to this supervisor
            var supervisorTeams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.SupervisorId == supervisorId);

            var teamIds = supervisorTeams.Select(t => t.Id).ToHashSet();

            // Then get all tasks in those teams
            var tasks = await _unitOfWork.Repository<TeamTasks>()
                .FindAsync(t => teamIds.Contains(t.TeamId));

            return tasks.Select(MapToTaskSummary);
        }

        // ─────────────────────────────────────────────────────
        // ADD COMMENT
        // ─────────────────────────────────────────────────────
        public async Task<TaskCommentResponseDTO> AddCommentAsync(int taskId, string userId, AddTaskCommentDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            var comment = new TaskComment
            {
                Content = dto.Content,
                UserId = userId,
                TaskId = taskId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TaskComment>().AddAsync(comment);
            await _unitOfWork.CompleteAsync();

            return new TaskCommentResponseDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                UserName = comment.User?.FullName ?? "Unknown",
                UserImage = comment.User?.Profile_Image,
                CreatedAt = comment.CreatedAt
            };
        }

        // ─────────────────────────────────────────────────────
        // DELETE COMMENT
        // User can only delete their own comment
        // ─────────────────────────────────────────────────────
        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            var comments = await _unitOfWork.Repository<TaskComment>()
                .FindAsync(c => c.Id == commentId && c.UserId == userId);

            var comment = comments.FirstOrDefault();
            if (comment == null) return false;

            _unitOfWork.Repository<TaskComment>().Delete(comment);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ═════════════════════════════════════════════════════
        // PRIVATE MAPPING HELPERS
        // ═════════════════════════════════════════════════════

        private static TaskResponseDTO MapToTaskResponse(TeamTasks task, Team? team) => new()
        {
            Id = task.Id,
            Details = task.Details,
            Deadline = task.Deadline,
            Status = task.Status,
            SolutionFile = task.SolutionFile,
            TeamId = task.TeamId,
            TeamName = team?.TeamName ?? "Unknown",
            AssignedStudentId = task.AssignedStudentId,
            AssignedStudentName = task.Student?.FullName,
            Comments = task.Comments?.Select(c => new TaskCommentResponseDTO
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                UserName = c.User?.FullName ?? "Unknown",
                UserImage = c.User?.Profile_Image,
                CreatedAt = c.CreatedAt
            }).ToList() ?? new()
        };

        private static TaskSummaryDTO MapToTaskSummary(TeamTasks task) => new()
        {
            Id = task.Id,
            Details = task.Details,
            Deadline = task.Deadline,
            Status = task.Status,
            TeamName = task.Team?.TeamName ?? "Unknown",
            AssignedStudentName = task.Student?.FullName
        };
    }
}
