using DomainLayer.DTOs;

namespace ServicesAbstractionLayer
{
    public interface IAuthService
    {
        Task<bool> SyncUserAsync(UserSyncDto dto);
    }
}
