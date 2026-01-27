using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IPostRepository
    {
            Task<IEnumerable<Post>> GetAllAsync();
            Task<Post?> GetByIdAsync(int id);

            Task<IEnumerable<Post>> GetPendingPostsAsync();
            Task<IEnumerable<Post>> GetPostsByCommunityAsync(int communityId);

            Task AddAsync(Post post);
            void Update(Post post);
            void Delete(Post post);

            Task SaveAsync();
        
    }
}
