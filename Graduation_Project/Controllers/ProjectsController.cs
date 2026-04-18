using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectsController(IProjectService projectService, IUnitOfWork unitOfWork)
        {
            _projectService = projectService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? category)
        {
            var response = await _projectService.GetAllProjectsAsync(search, category);
            return Ok(response);
        }

        [HttpGet("my-projects/{studentId}")]
        public async Task<IActionResult> GetMyProjects(string studentId)
        {
            var projects = await _projectService.GetMyProjectsAsync(studentId);
            return Ok(projects);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] StudentProjectCreateDto dto)
        {
            if (dto.CoverPhoto == null) return BadRequest("Cover photo is required");

            var success = await _projectService.UploadStudentProjectAsync(dto);
            if (success) return Ok(new { message = "Project uploaded successfully!" });

            return BadRequest("Failed to upload project");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string studentId)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
            if (project == null) return NotFound();

            if (project.StudentId != studentId)
                return Forbid("You are not allowed to delete this project");

            _unitOfWork.Repository<Project>().Delete(project);
            await _unitOfWork.CompleteAsync();
            return Ok(new { message = "Project deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _unitOfWork.Repository<Project>().GetEntityWithSpec(
                p => p.Id == id,
                p => p.Student
            );
            if (project == null)
            {
                return NotFound(new { message = "project not found" });
            }
            return Ok(project);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProjectUpdateDto dto)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
            if (project == null) return NotFound();

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.TechnologyUsed = dto.Tags;
            project.Category = dto.Category;
            project.ProjectFilePath = dto.GitHubUrl;
            if (dto.CoverPhoto != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");

                if (!string.IsNullOrEmpty(project.ImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", project.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string imageFileName = Guid.NewGuid() + "_" + dto.CoverPhoto.FileName;
                string imagePath = Path.Combine(uploadsFolder, imageFileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await dto.CoverPhoto.CopyToAsync(stream);
                }
                project.ImageUrl = "/uploads/images/" + imageFileName;
            }

            _unitOfWork.Repository<Project>().Update(project);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Project updated successfully" });
        }

        [HttpGet("{projectId}/team-members")]
        public async Task<IActionResult> GetTeamMembers(int projectId)
        {
            var members = await _projectService.GetProjectTeamMembersAsync(projectId);

            if (members == null)
                return NotFound(new { message = "Project not found" });

            return Ok(members);
        }

    }
}
