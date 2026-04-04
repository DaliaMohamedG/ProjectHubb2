using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _unitOfWork.Repository<Project>().GetAllAsync();
        }
        public async Task<bool> UploadStudentProjectAsync(StudentProjectCreateDto dto)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            string imagesFolder = Path.Combine(uploadsFolder, "images");
            string docsFolder = Path.Combine(uploadsFolder, "documents");

            if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
            if (!Directory.Exists(docsFolder)) Directory.CreateDirectory(docsFolder);

            string imageFileName = Guid.NewGuid() + "_" + dto.CoverPhoto.FileName;
            string imagePath = Path.Combine(imagesFolder, imageFileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await dto.CoverPhoto.CopyToAsync(stream);
            }

            string? docFileName = null;
            if (dto.ProjectDocument != null)
            {
                docFileName = Guid.NewGuid() + "_" + dto.ProjectDocument.FileName;
                string docPath = Path.Combine(docsFolder, docFileName);
                using (var stream = new FileStream(docPath, FileMode.Create))
                {
                    await dto.ProjectDocument.CopyToAsync(stream);
                }
            }

            var newProject = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                TechnologyUsed = dto.Tags,
                StudentId = dto.StudentId,
                ProjectFilePath = docFileName != null ? "/uploads/documents/" + docFileName : dto.GitHubUrl,
                ImageUrl = "/uploads/images/" + imageFileName
            };

            await _unitOfWork.Repository<Project>().AddAsync(newProject);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
