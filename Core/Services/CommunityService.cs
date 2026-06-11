using DomainLayer.Contracts;
using DomainLayer.DTOs;
using DomainLayer.DTOs.CommunityDtos;
using DomainLayer.Models;
using ServicesAbstractionLayer;
using SixLabors.ImageSharp;

namespace ServicesLayer
{
    public class CommunityService : ICommunityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CommunityService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }



        public async Task<IEnumerable<PostResponseDto>> GetTimelinePostsAsync(string currentUserId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.Visibility == "public",
                p => p.User,
                p => p.Likes,
                p => p.Comments
            );

            return posts.Select(p => MapToPostDto(p, currentUserId)).ToList();
        }
        public async Task<IEnumerable<PostResponseDto>> GetTeamPostsAsync(int teamId, string currentUserId)
        {
            var isMember = await _unitOfWork.Repository<TeamMember>().FindAsync(
                tm => tm.TeamId == teamId && tm.UserId == currentUserId
            );

            if (isMember == null || !isMember.Any())
            {
                Console.WriteLine($"Access Denied: User {currentUserId} is not a member of Team {teamId}");
                return new List<PostResponseDto>();
            }

            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.TeamId == teamId,
                p => p.User,
                p => p.Likes,
                p => p.Comments
            );

            if (posts == null || !posts.Any())
            {
                return new List<PostResponseDto>();
            }

            return posts.Select(p => MapToPostDto(p, currentUserId)).ToList();
        }
        public async Task<IEnumerable<PostResponseDto>> GetMyPostsAsync(string userId)
        {
            var posts = await _unitOfWork.Repository<Post>().ListWithSpec(
                p => p.UserId == userId,
                p => p.User,
                p => p.Likes,
                p => p.Comments
            );

            return posts.Select(p => MapToPostDto(p, userId)).ToList();
        }
        public async Task<bool> CreatePostAsync(PostCreateDto dto)
        {
            string? imagePath = null;

            if (dto.PostImage != null && dto.PostImage.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}.jpg";
                string fullPath = Path.Combine(folder, fileName);

                using (var inputStream = dto.PostImage.OpenReadStream())
                {
                    using (var image = await Image.LoadAsync(inputStream))
                    {
                        await image.SaveAsJpegAsync(fullPath);
                    }
                }
                imagePath = "/uploads/posts/" + fileName;
            }

            var post = new Post
            {
                Content = dto.Content,
                Visibility = dto.Visibility,
                UserId = dto.UserId,
                TeamId = (dto.TeamId == 0) ? null : dto.TeamId,
                ImageUrl = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Post>().AddAsync(post);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(int postId)
        {
            var comments = await _unitOfWork.Repository<PostComment>().ListWithSpec(
  c => c.PostId == postId && c.ParentCommentId == null,
    c => c.User,
    c => c.Replies
);
            var baseUrl = "https://projecthubb.runasp.net";
            return comments.Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id.ToString(),
                    UserName = c.User?.FullName ?? "Unknown User",
                    UserImage = string.IsNullOrEmpty(c.User.Profile_Image) ? null : baseUrl + c.User.Profile_Image,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,

                    Replies = c.Replies?.Select(r => new CommentResponseDto
                    {
                        Id = r.Id.ToString(),
                        Content = r.Content,
                        UserName = r.User?.FullName ?? "Unknown User",
                        UserImage = string.IsNullOrEmpty(r.User.Profile_Image) ? null : baseUrl + r.User.Profile_Image,
                        CreatedAt = r.CreatedAt
                    }).ToList() ?? new List<CommentResponseDto>()
                }).ToList();
        }
        public async Task<bool> AddCommentAsync(CommentCreateDto dto)
        {
            var comment = new PostComment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                UserId = dto.UserId,
                ParentCommentId = dto.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PostComment>().AddAsync(comment);
            var result = await _unitOfWork.CompleteAsync() > 0;

            if (result)
            {
                var userWhoCommented = await _unitOfWork.Repository<User>().GetByIdAsync(dto.UserId);
                string commenterName = userWhoCommented?.FullName ?? "Someone";

                if (dto.ParentCommentId != null)
                {
                    var parentComment = await _unitOfWork.Repository<PostComment>().GetByIdAsync(dto.ParentCommentId.Value);

                    if (parentComment != null && parentComment.UserId != dto.UserId)
                    {
                        try
                        {
                            await _notificationService.SendToUserAsync(
                                parentComment.UserId,
                                "New reply to your comment!",
                                $"{commenterName} replied to your comment.",
                                "info"
                            );
                        }
                        catch { }
                    }
                }

                var post = await _unitOfWork.Repository<Post>().GetByIdAsync(dto.PostId);

                if (post != null && post.UserId != dto.UserId)
                {
                    try
                    {
                        await _notificationService.SendToUserAsync(
                            post.UserId,
                            "New comment on your post!",
                            $"{commenterName} commented on your post.",
                            "info"
                        );
                    }
                    catch { }
                }
            }

            return result;
        }
        public async Task<bool> ToggleLikeAsync(int postId, string userId)
        {
            var repo = _unitOfWork.Repository<Like>();
            var existingLike = await repo.GetEntityWithSpec(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
                repo.Delete(existingLike);
            else
            {
                await repo.AddAsync(new Like { PostId = postId, UserId = userId });
                var post = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
                if (post != null && post.UserId != userId)
                {
                    await _notificationService.SendToUserAsync(
            post.UserId,
            "new like",
            $"{user.FullName} like your post",
            "success"
        );
                }
            }
            return await _unitOfWork.CompleteAsync() > 0;
        }
        private PostResponseDto MapToPostDto(Post p, string currentUserId)
        {
            var baseUrl = "https://projecthubb.runasp.net";
            return new PostResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = p.User?.FullName ?? "Unknown User",
                UserImage = string.IsNullOrEmpty(p.User.Profile_Image) ? null : baseUrl + p.User.Profile_Image,
                UserAvatarColor = "#DBEAFE",
                Content = p.Content,
                TimeAgo = p.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                Likes = p.Likes?.Count() ?? 0,
                CommentsCount = p.Comments?.Count() ?? 0,
                IsLiked = p.Likes?.Any(l => l.UserId == currentUserId) ?? false,
                LikedByUserIds = p.Likes?.Select(l => l.UserId).ToList() ?? new List<string>(),
                PostImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? null : baseUrl + p.ImageUrl,
                Visibility = p.Visibility?.ToLower() ?? "public"
            };
        }
        public async Task<bool> DeletePostAsync(int postId, string userId)
        {
            var posts = await _unitOfWork.Repository<Post>()
                .FindAsync(p => p.Id == postId && p.UserId == userId);

            var post = posts.FirstOrDefault();
            if (post == null) return false;

            _unitOfWork.Repository<Post>().Delete(post);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _unitOfWork.Repository<PostComment>().GetByIdAsync(commentId);

            if (comment == null)
            {
                return false;
            }

            if (comment.UserId != userId)
            {
                return false;
            }

            _unitOfWork.Repository<PostComment>().Delete(comment);

            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}