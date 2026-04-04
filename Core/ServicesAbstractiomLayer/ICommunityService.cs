using DomainLayer.DTOs;
using DomainLayer.Models;

namespace ServicesAbstractionLayer
{
    public interface ICommunityService
    {
        Task<IEnumerable<PostResponseDto>> GetTimelinePostsAsync(string currentUserId);
        Task<IEnumerable<PostResponseDto>> GetTeamPostsAsync(int teamId, string currentUserId);
        Task<IEnumerable<PostResponseDto>> GetMyPostsAsync(string userId);
        Task<bool> CreatePostAsync(PostCreateDto dto);
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<bool> AddCommentAsync(CommentCreateDto dto);
        Task<bool> ToggleLikeAsync(int postId, string userId);
    }
}
