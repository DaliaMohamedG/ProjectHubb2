using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace PersistenceLayer.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
            => await _context.Posts
            .Include(p => p.Comments)
            .ToListAsync();

        public async Task<Post?> GetByIdAsync(string id)
            => await _context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);

        public Task<IEnumerable<Post>> GetPendingPostsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetPostsByCommunityAsync(int communityId)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(Post post)
           => await _context.Posts.AddAsync(post);

        public void Delete(Post post)
            => _context.Posts.Update(post);

        public async Task SaveAsync()
            => await _context.SaveChangesAsync();

        public void Update(Post post)
            => _context.Posts.Update(post);
    }
}
