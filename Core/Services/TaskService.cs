using DomainLayer.Contracts;
using DomainLayer.DTOs.Task_Dtos;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;

        public TaskService(IUnitOfWork unitOfWork, NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        // ─────────────────────────────────────────────────────
        // CREATE TASK
        // Only supervisor of the team can create a task
        // ─────────────────────────────────────────────────────
        public async Task<TaskResponseDTO> CreateTaskAsync(string supervisorId, CreateTaskDTO dto)
        {
            // Step 1: Check the team exists AND belongs to this supervisor
            // استخدمي Include(t => t.Members) عشان نأكد وجود الأعضاء في الذاكرة
            var teams = await _unitOfWork.Repository<Team>()
                .ListWithSpec(t => t.Id == dto.TeamId && t.SupervisorId == supervisorId, t => t.Members);

            if (!teams.Any())
                throw new UnauthorizedAccessException("You can only assign tasks to your own teams.");

            var team = teams.First();

            // Step 2: التصليح هنا
            if (dto.AssignedTo != null && dto.AssignedTo.Any())
            {
                // بنلف جوه الـ Members ونشوف هل فيه حد الـ UserId بتاعه بيساوي الـ Id اللي مبعوت
                bool studentInTeam = dto.AssignedTo.All(studentId => team.Members.Any(m => m.UserId == studentId));

                if (!studentInTeam)
                    throw new InvalidOperationException("This student is not a member of this team.");
            }

            // Step 3: Create the task
            var task = new TeamTasks
            {
                Id = Guid.NewGuid().ToString(),  // ← ADD THIS
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = false,
                TeamId = dto.TeamId
            };

            // Step 4: Save
            await _unitOfWork.Repository<TeamTasks>().AddAsync(task);
            await _unitOfWork.CompleteAsync();

            // Step 5: Save assignments (one row per assigned student)
            if (dto.AssignedTo != null && dto.AssignedTo.Any())
            {
                foreach (var studentId in dto.AssignedTo)
                {
                    var assignment = new TaskAssignment
                    {
                        Id = Guid.NewGuid().ToString(),
                        TaskId = task.Id,
                        StudentId = studentId
                    };
                    await _unitOfWork.Repository<TaskAssignment>().AddAsync(assignment);
                }
                await _unitOfWork.CompleteAsync();
            }

            return MapToTaskResponse(task, team);
        }

        // ─────────────────────────────────────────────────────
        // UPDATE TASK
        // ─────────────────────────────────────────────────────
        public async Task<TaskResponseDTO?> UpdateTaskAsync(string taskId, string supervisorId, UpdateTaskDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return null;

            // Check this supervisor owns the team
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId && t.SupervisorId == supervisorId);
            if (!teams.Any()) return null;

            // Only update fields that were actually sent
            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.DueDate.HasValue) task.DueDate = dto.DueDate.Value;

            if (dto.AssignedTo != null && dto.AssignedTo.Any())
            {
                // Remove old assignments first
                var oldAssignments = await _unitOfWork.Repository<TaskAssignment>()
                    .FindAsync(a => a.TaskId == taskId);
                foreach (var old in oldAssignments)
                    _unitOfWork.Repository<TaskAssignment>().Delete(old);

                // Add new assignments
                foreach (var studentId in dto.AssignedTo)
                {
                    await _unitOfWork.Repository<TaskAssignment>().AddAsync(new TaskAssignment
                    {
                        Id = Guid.NewGuid().ToString(),
                        TaskId = taskId,
                        StudentId = studentId
                    });
                }
            }
            _unitOfWork.Repository<TeamTasks>().Update(task);
            await _unitOfWork.CompleteAsync();

            return MapToTaskResponse(task, teams.First());
        }

        // ─────────────────────────────────────────────────────
        // DELETE TASK
        // ─────────────────────────────────────────────────────
        public async Task<bool> DeleteTaskAsync(string taskId, string supervisorId)
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
        public async Task<bool> SubmitTaskAsync(string taskId, string studentId, SubmitTaskDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return false;

            // 1.
            var assignments = await _unitOfWork.Repository<TaskAssignment>()
                .FindAsync(a => a.TaskId == taskId);

            // If task has specific assignments → only those students can submit
            if (assignments.Any() && !assignments.Any(a => a.StudentId == studentId))
                return false;

            // 2. التأكد إن الطالب عضو في التيم
            // نستخدم ListWithSpec عشان نجيب الـ Members معانا في الـ Query
            var teams = await _unitOfWork.Repository<Team>()
                .ListWithSpec(t => t.Id == task.TeamId, t => t.Members);

            var team = teams.FirstOrDefault();

            // التصليح هنا: نستخدم Members والـ UserId
            if (team == null || !team.Members.Any(m => m.UserId == studentId))
                return false;

            // Save attachments
            if (dto.StudentAttachments != null && dto.StudentAttachments.Any())
            {
                foreach (var file in dto.StudentAttachments)
                {
                    await _unitOfWork.Repository<TaskAttachment>().AddAsync(new TaskAttachment
                    {
                        Name = file.Name,
                        Type = file.Type,
                        FilePath = file.Name,     // store name as path for now
                        UploadedBy = "Student",
                        TaskId = taskId
                    });
                }
            }
            task.Status = true;

            _unitOfWork.Repository<TeamTasks>().Update(task);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ─────────────────────────────────────────────────────
        // GIVE FEEDBACK
        // Supervisor updates task status after reviewing
        // ─────────────────────────────────────────────────────
        public async Task<bool> GiveFeedbackAsync(string taskId, string supervisorId, TaskFeedbackDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null) return false;

            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == task.TeamId && t.SupervisorId == supervisorId);
            if (!teams.Any()) return false;

            task.Status = true;
            _unitOfWork.Repository<TeamTasks>().Update(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────
        // GET TASK BY ID (full details with comments)
        // ─────────────────────────────────────────────────────
        public async Task<TaskResponseDTO?> GetTaskByIdAsync(string taskId)
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
            // Step 1: get all task IDs assigned to this student
            var assignments = await _unitOfWork.Repository<TaskAssignment>()
                .FindAsync(a => a.StudentId == studentId);

            var taskIds = assignments.Select(a => a.TaskId).ToHashSet();

            // Step 2: get those tasks
            var tasks = await _unitOfWork.Repository<TeamTasks>()
                .FindAsync(t => taskIds.Contains(t.Id));

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
        public async Task<TaskCommentResponseDTO> AddCommentAsync(string taskId, string userId, AddTaskCommentDTO dto)
        {
            var task = await _unitOfWork.Repository<TeamTasks>().GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            var comment = new TaskComment
            {
                Content = dto.Text,
                UserId = userId,
                TaskId = taskId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TaskComment>().AddAsync(comment);
            await _unitOfWork.CompleteAsync();

            return new TaskCommentResponseDTO
            {
                Id = comment.Id,
                Text = comment.Content,
                UserId = comment.UserId,
                UserName = comment.User?.FullName ?? "Unknown",
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
            // FIX: all field names updated to match Flutter ✅
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            From = "Supervisor",
            DueDate = task.DueDate,
            TeamId = task.TeamId.ToString(),
            SupervisorId = team?.SupervisorId ?? "",
            // FIX: get assigned students from Assignments collection ✅
            AssignedTo = task.Assignments?
                .Select(a => a.StudentId)
                .ToList() ?? new(),
            // FIX: Status is bool ✅
            IsCompleted = task.Status,
            SupervisorAttachments = task.Attachments?
                .Where(a => a.UploadedBy == "Supervisor")
                .Select(a => new AttachmentDTO { Name = a.Name, Type = a.Type })
                .ToList() ?? new(),
            StudentAttachments = task.Attachments?
                .Where(a => a.UploadedBy == "Student")
                .Select(a => new AttachmentDTO { Name = a.Name, Type = a.Type })
                .ToList() ?? new()
        };

        private static TaskSummaryDTO MapToTaskSummary(TeamTasks task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            IsCompleted = task.Status,
            TeamId = task.TeamId.ToString(),
            AssignedTo = task.Assignments?.FirstOrDefault()?.Student?.FullName
        };
    }
}
