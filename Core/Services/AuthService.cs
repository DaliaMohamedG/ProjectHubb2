using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> SyncUserAsync(UserSyncDto dto)
        {
            var incomingRole = dto.Role.ToLower();

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
                    Profile_Image = dto.Picture,
                    Role = dto.Role

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
                    Profile_Image = dto.Picture,
                    Role = dto.Role

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
                    Profile_Image = dto.Picture,
                    Role = dto.Role

                };
                await _unitOfWork.Repository<Assistant>().AddAsync(assistant);
            }

            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
    }
}
