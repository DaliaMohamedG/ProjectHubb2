using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;
using SixLabors.ImageSharp;

namespace ServicesLayer
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> SyncUserAsync(UserSyncDto dto)
        {
            var incomingRole = dto.Role.ToLower();

            string? savedImagePath = null;

            if (dto.Picture != null && dto.Picture.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string fileName = $"{Guid.NewGuid()}.jpg";
                string fullPath = Path.Combine(folderPath, fileName);

                using (var inputStream = dto.Picture.OpenReadStream())
                {
                    using var image = await Image.LoadAsync(inputStream);
                    await image.SaveAsJpegAsync(fullPath);
                }
                savedImagePath = $"/uploads/users/{fileName}";
            }
            if (incomingRole == "user" || incomingRole == "student")
            {
                var student = new Student
                {
                    Id = dto.Id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Instituation = dto.Instituation,
                    Track = dto.Track,
                    Faculty = dto.Faculty,
                    Profile_Image = savedImagePath,
                    Role = incomingRole
                };
                await _unitOfWork.Repository<Student>().AddAsync(student);
            }

            else if (incomingRole == "supervisor" || incomingRole == "doctor")
            {
                var supervisor = new Supervisor
                {
                    Id = dto.Id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Instituation = dto.Instituation,
                    Faculty = dto.Faculty,
                    Profile_Image = savedImagePath,
                    Role = incomingRole
                };
                await _unitOfWork.Repository<Supervisor>().AddAsync(supervisor);
            }
            else if (incomingRole == "assistant")
            {
                var assistant = new Assistant
                {
                    Id = dto.Id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Instituation = dto.Instituation,
                    Faculty = dto.Faculty,
                    Profile_Image = savedImagePath,
                    Role = incomingRole
                };
                await _unitOfWork.Repository<Assistant>().AddAsync(assistant);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result >= 0;
        }
        public async Task<string> GetUserProfileImageAsync(string userId)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            var baseUrl = "https://projecthubb.runasp.net";

            if (user == null) return null;
            return string.IsNullOrEmpty(user.Profile_Image) ? null : baseUrl + user.Profile_Image;
        }
        public async Task<bool> EditUserProfileAsync(UserEditDto dto)
        {
            var student = await _unitOfWork.Repository<Student>().GetByIdAsync(dto.Id);
            var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(dto.Id);
            var assistant = await _unitOfWork.Repository<Assistant>().GetByIdAsync(dto.Id);

            if (student == null && supervisor == null && assistant == null) return false;

            string? savedImagePath = null;
            if (dto.Picture != null && dto.Picture.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string fileName = $"{Guid.NewGuid()}.jpg";
                string fullPath = Path.Combine(folderPath, fileName);

                using (var inputStream = dto.Picture.OpenReadStream())
                using (var image = await Image.LoadAsync(inputStream))
                {
                    await image.SaveAsJpegAsync(fullPath);
                }
                savedImagePath = $"/uploads/users/{fileName}";
            }

            if (student != null)
            {
                student.FullName = dto.FullName ?? student.FullName;
                student.Instituation = dto.University ?? student.Instituation;
                if (savedImagePath != null) student.Profile_Image = savedImagePath;
                _unitOfWork.Repository<Student>().Update(student);
            }
            else if (supervisor != null)
            {
                supervisor.FullName = dto.FullName ?? supervisor.FullName;
                supervisor.Instituation = dto.University ?? supervisor.Instituation;
                if (savedImagePath != null) supervisor.Profile_Image = savedImagePath;
                _unitOfWork.Repository<Supervisor>().Update(supervisor);
            }
            else if (assistant != null)
            {
                assistant.FullName = dto.FullName ?? assistant.FullName;
                assistant.Instituation = dto.University ?? assistant.Instituation;
                if (savedImagePath != null) assistant.Profile_Image = savedImagePath;
                _unitOfWork.Repository<Assistant>().Update(assistant);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
    }
}
