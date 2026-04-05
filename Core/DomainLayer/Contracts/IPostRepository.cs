using DomainLayer.Models;

namespace DomainLayer.Contracts
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(string id);

        Task<IEnumerable<Post>> GetPendingPostsAsync();
        Task<IEnumerable<Post>> GetPostsByCommunityAsync(int communityId);

        Task AddAsync(Post post);
        void Update(Post post);
        void Delete(Post post);

        Task SaveAsync();

    }
}
