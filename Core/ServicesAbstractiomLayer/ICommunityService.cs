using DomainLayer.DTOs;
using DomainLayer.DTOs.CommunityDtos;

namespace ServicesAbstractionLayer
{
    public interface ICommunityService
    {
        Task<IEnumerable<PostResponseDto>> GetTimelinePostsAsync(string currentUserId);
        Task<IEnumerable<PostResponseDto>> GetTeamPostsAsync(int teamId, string currentUserId);
        Task<IEnumerable<PostResponseDto>> GetMyPostsAsync(string userId);
        Task<bool> CreatePostAsync(PostCreateDto dto);
        Task<bool> DeletePostAsync(int postId, string userId);
        Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(int postId);
        Task<bool> AddCommentAsync(CommentCreateDto dto);
        Task<bool> DeleteCommentAsync(int commentId, string userId);
        Task<bool> ToggleLikeAsync(int postId, string userId);
    }
}
