using DomainLayer.DTOs.Task_Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractionLayer
{
    public interface ITaskService
    {
        // ── TASK OPERATIONS ──────────────────────────────────

        // Supervisor creates a new task
        Task<TaskResponseDTO> CreateTaskAsync(string supervisorId, CreateTaskDTO dto);

        // Supervisor updates a task
        Task<TaskResponseDTO?> UpdateTaskAsync(string taskId, string supervisorId, UpdateTaskDTO dto);

        // Supervisor deletes a task
        Task<bool> DeleteTaskAsync(string taskId, string supervisorId);

        // Student submits their work
        Task<bool> SubmitTaskAsync(string taskId, string studentId, SubmitTaskDTO dto);

        // Supervisor gives feedback (approves or rejects)
        Task<bool> GiveFeedbackAsync(string taskId, string supervisorId, TaskFeedbackDTO dto);

        // ── QUERY OPERATIONS ─────────────────────────────────

        // Get one task by its ID (Task Details screen)
        Task<TaskResponseDTO?> GetTaskByIdAsync(string taskId);

        // Get all tasks for a team (Team Tasks screen - Supervisor)
        Task<IEnumerable<TaskSummaryDTO>> GetTasksByTeamAsync(int teamId);

        // Get all tasks for a student (My Work screen - Student)
        Task<IEnumerable<TaskSummaryDTO>> GetTasksByStudentAsync(string studentId);

        // Get all tasks across all supervisor's teams (All Tasks screen)
        Task<IEnumerable<TaskSummaryDTO>> GetTasksBySupervisorAsync(string supervisorId);

        // ── COMMENT OPERATIONS ───────────────────────────────

        // Anyone adds a comment on a task
        Task<TaskCommentResponseDTO> AddCommentAsync(string taskId, string userId, AddTaskCommentDTO dto);

        // User deletes their own comment
        Task<bool> DeleteCommentAsync(int commentId, string userId);
    }
}
