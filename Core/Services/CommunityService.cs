using DomainLayer.Contracts;
using DomainLayer.DTOs.CommunityDtos;
using DomainLayer.DTOs.PostDtos;
using DomainLayer.Models;
using ServicesAbstractionLayer;
using System.Xml.Linq;

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
                p => p.Likes,
                p => p.Comments
            );

            return posts.Select(p => MapToPostResponse(p, currentUserId)).ToList();
        }
        public async Task<IEnumerable<PostResponseDto>> GetTeamPostsAsync(int teamId, string currentUserId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.Visibility == "My Team" && p.TeamId == teamId,
                p => p.User,
                p => p.Likes,
                p => p.Comments
            );

            return posts.Select(p => MapToPostResponse(p, currentUserId)).ToList();
        }
        public async Task<IEnumerable<PostResponseDto>> GetMyPostsAsync(string userId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.UserId == userId,
                p => p.User,
                p => p.Likes,
                p => p.Comments
            );

            return posts.Select(p => MapToPostResponse(p, userId)).ToList();
        }
        public async Task<bool> CreatePostAsync(PostCreateDto dto, string userId)
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
                UserId = userId,                 // comes from JWT token
                ImageUrl = imagePath,
                TeamId = dto.TeamId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Post>().AddAsync(post);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeletePostAsync(string postId, string userId)
        {
            var posts = await _unitOfWork.Repository<Post>()
                .FindAsync(p => p.Id == postId && p.UserId == userId);

            var post = posts.FirstOrDefault();
            if (post == null) return false;

            _unitOfWork.Repository<Post>().Delete(post);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<IEnumerable<CommentResponseDto>> GetCommentsByPostIdAsync(int postId)
        {
            var comments = await _unitOfWork.Repository<PostComment>().FindAsync(c => c.PostId == postId);

            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                PostId = c.PostId.ToString(),
                UserName = c.User?.FullName ?? "Unknown",
                UserInitial = c.User?.FullName?.FirstOrDefault().ToString() ?? "?",
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Likes = 0,                              // add likes to Comment model later
                IsLiked = false
            }).ToList();
        }
        public async Task<bool> AddCommentAsync(CommentCreateDto dto,string userId)
        {
            var comment = new PostComment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<PostComment>().AddAsync(comment);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteCommentAsync(string commentId, string userId)
        {
            var comments = await _unitOfWork.Repository<PostComment>()
                .FindAsync(c => c.Id == commentId && c.UserId == userId);

            var comment = comments.FirstOrDefault();
            if (comment == null) return false;

            _unitOfWork.Repository<PostComment>().Delete(comment);
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

        private static PostResponseDto MapToPostResponse(Post post, string currentUserId)
        {
            // Get list of user IDs who liked this post
            var likedByUserIds = post.Likes?
                .Select(l => l.UserId)
                .ToList() ?? new();

            return new PostResponseDto
            {
                Id = post.Id.ToString(),
                UserId = post.UserId,
                UserName = post.User?.FullName ?? "Unknown",
                UserAvatarColor = "#DBEAFE",                    // default color for now
                CreatedAt = post.CreatedAt,                     // Flutter calculates timeAgo
                Content = post.Content,
                Hashtags = ExtractHashtags(post.Content),       // extract from content
                LikesCount = post.Likes?.Count ?? 0,                 // just the count
                CommentsCount = post.Comments?.Count ?? 0,
                AttachmentName = post.ImageUrl,                 // ImageUrl → AttachmentName
                IsLiked = likedByUserIds.Contains(currentUserId),
                LikedByUserIds = likedByUserIds,
                Visibility = post.Visibility?.ToLower() ?? "public"
            };
        }


        private static List<string> ExtractHashtags(string content)
        {
            if (string.IsNullOrEmpty(content)) return new();

            return content
                .Split(' ')
                .Where(word => word.StartsWith("#"))
                .ToList();
        }
    }
}