using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.Models;
using ServicesAbstractionLayer;

namespace ServicesLayer
{
    public class CommunityService : ICommunityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PostResponseDto>> GetTimelinePostsAsync(string currentUserId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.Visibility == "Public",
                p => p.User,
                p => p.Likes
            );

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                LikesCount = p.Likes.Count(),
                IsLiked = p.Likes.Any(l => l.UserId == currentUserId)
            }).ToList();
        }
        public async Task<IEnumerable<PostResponseDto>> GetTeamPostsAsync(int teamId, string currentUserId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.Visibility == "My Team" && p.TeamId == teamId,
                p => p.User,
                p => p.Likes
            );

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                LikesCount = p.Likes.Count(),
                IsLiked = p.Likes.Any(l => l.UserId == currentUserId)
            }).ToList();
        }
        public async Task<IEnumerable<PostResponseDto>> GetMyPostsAsync(string userId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.UserId == userId,
                p => p.User,
                p => p.Likes
            );

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                LikesCount = p.Likes.Count(),
                IsLiked = p.Likes.Any(l => l.UserId == userId) 
            }).ToList();
        }
        public async Task<bool> CreatePostAsync(PostCreateDto dto)
        {
            string? imagePath = null;

            if (dto.PostImage != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/posts");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.PostImage.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.PostImage.CopyToAsync(stream);
                }
                imagePath = "/uploads/posts/" + fileName;
            }

            var post = new Post
            {
                Content = dto.Content,
                Visibility = dto.Visibility,
                UserId = dto.UserId,
                ImageUrl = imagePath,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Post>().AddAsync(post);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _unitOfWork.Repository<Comment>().FindAsync(c => c.PostId == postId);
        }
        public async Task<bool> AddCommentAsync(CommentCreateDto dto)
        {
            var comment = new Comment
            {
                Text = dto.Text,
                PostId = dto.PostId,
                UserId = dto.UserId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Comment>().AddAsync(comment);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<bool> ToggleLikeAsync(int postId, string userId)
        {
            var repo = _unitOfWork.Repository<Like>();
            var existingLike = await repo.GetEntityWithSpec(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
                repo.Delete(existingLike);
            else
                await repo.AddAsync(new Like { PostId = postId, UserId = userId });

            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}