using DomainLayer.DTOs;

namespace ServicesAbstractionLayer
{
    public interface IAuthService
    {
        Task<bool> SyncUserAsync(UserSyncDto dto);
        Task<string> GetUserProfileImageAsync(string userId);
        Task<bool> EditUserProfileAsync(UserEditDto dto);
    }
}
