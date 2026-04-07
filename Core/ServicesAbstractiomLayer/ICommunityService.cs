using DomainLayer.DTOs;
using DomainLayer.DTOs.PostDtos;
using DomainLayer.Models;

namespace ServicesAbstractionLayer
{
    public interface ICommunityService
    {
        Task<IEnumerable<PostResponseDto>> GetTimelinePostsAsync(string currentUserId);
        Task<IEnumerable<PostResponseDto>> GetTeamPostsAsync(int teamId, string currentUserId);
        Task<IEnumerable<PostResponseDto>> GetMyPostsAsync(string userId);
        Task<bool> CreatePostAsync(PostCreateDto dto);
        Task<bool> DeletePostAsync(string postId, string userId);
        Task<IEnumerable<CommentResponseDto>> GetCommentsByPostIdAsync(int postId);
        Task<bool> AddCommentAsync(CommentCreateDto dto , string userId);
        Task<bool> DeleteCommentAsync(string commentId, string userId);
        Task<bool> ToggleLikeAsync(int postId, string userId);
    }
}
