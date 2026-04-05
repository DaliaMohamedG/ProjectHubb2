using DomainLayer.DTOs.Task_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;
using System.Security.Claims;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // ── reads the current user's ID from the JWT token ──
        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;


        // ═══════════════════════════════════════════════════════
        // TASK ENDPOINTS
        // ═══════════════════════════════════════════════════════

        // Get full task details (Task Details screen)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
                return NotFound(new { message = "Task not found." });

            return Ok(task);
        }


        // Get all tasks for a team (Team Tasks screen - Supervisor)
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetTasksByTeam(int teamId)
        {
            var tasks = await _taskService.GetTasksByTeamAsync(teamId);
            return Ok(tasks);
        }

        // Student gets their own assigned tasks (My Work screen)
        [HttpGet("my-tasks")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyTasks()
        {
            var tasks = await _taskService.GetTasksByStudentAsync(CurrentUserId);
            return Ok(tasks);
        }

        // Supervisor gets all tasks across their teams (All Tasks screen)
        [HttpGet("all-tasks")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetAllTasksBySupervisor()
        {
            var tasks = await _taskService.GetTasksBySupervisorAsync(CurrentUserId);
            return Ok(tasks);
        }

        // Supervisor creates/assigns a new task (Assign Task screen)
        [HttpPost]
        //[Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var task = await _taskService.CreateTaskAsync(CurrentUserId, dto);

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Supervisor tried to assign to a team that's not theirs
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Student not in the team
                return BadRequest(new { message = ex.Message });
            }
        }

        // Supervisor updates a task
        [HttpPut("{id}")]
        //[Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] UpdateTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _taskService.UpdateTaskAsync(id, CurrentUserId, dto);

            if (result == null)
                return NotFound(new { message = "Task not found or you don't have permission." });

            return Ok(result);
        }

        // Supervisor deletes a task
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var success = await _taskService.DeleteTaskAsync(id, CurrentUserId);

            if (!success)
                return NotFound(new { message = "Task not found or you don't have permission." });

            return Ok(new { message = "Task deleted successfully." });
        }

        // Student submits their solution (Submit Task screen)
        [HttpPost("{id}/submit")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitTask(string id, [FromBody] SubmitTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _taskService.SubmitTaskAsync(id, CurrentUserId, dto);

            if (!success)
                return BadRequest(new { message = "Could not submit. Check task ID or your team membership." });

            return Ok(new { message = "Task submitted successfully." });
        }

        // Supervisor gives feedback (Feedback screen)
        // Updates task status to "Done" or "NeedsRevision"
        [HttpPost("{id}/feedback")]
        //[Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GiveFeedback(string id, [FromBody] TaskFeedbackDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _taskService.GiveFeedbackAsync(id, CurrentUserId, dto);

            if (!success)
                return NotFound(new { message = "Task not found or you don't have permission." });

            return Ok(new { message = "Feedback submitted successfully." });
        }


        // ═══════════════════════════════════════════════════════
        // COMMENT ENDPOINTS
        // ═══════════════════════════════════════════════════════

        // Any user adds a comment on a task (Task Details screen)
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(string id, [FromBody] AddTaskCommentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var comment = await _taskService.AddCommentAsync(id, CurrentUserId, dto);
                return Ok(comment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // User deletes their own comment
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var success = await _taskService.DeleteCommentAsync(commentId, CurrentUserId);

            if (!success)
                return NotFound(new { message = "Comment not found or you don't own it." });

            return Ok(new { message = "Comment deleted successfully." });
        }
    }
}

