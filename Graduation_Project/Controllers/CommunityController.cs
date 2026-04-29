using DomainLayer.DTOs;
using DomainLayer.DTOs.CommunityDtos;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractionLayer;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetTimeline(string currentUserId)
        {
            var posts = await _communityService.GetTimelinePostsAsync(currentUserId);
            return Ok(posts);
        }

        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetTeamPosts(int teamId, string currentUserId)
        {
            var posts = await _communityService.GetTeamPostsAsync(teamId, currentUserId);
            return Ok(posts);
        }

        [HttpGet("my-posts/{userId}")]
        public async Task<IActionResult> GetUserPosts(string userId)
        {
            var posts = await _communityService.GetMyPostsAsync(userId);
            return Ok(posts);
        }

        [HttpPost("posts")]
        public async Task<IActionResult> Create([FromForm] PostCreateDto dto)
        {
            var result = await _communityService.CreatePostAsync(dto);
            if (!result) return BadRequest(new { message = "Error creating post" });
            return Ok(new { message = "Successfully published" });
        }

        [HttpGet("posts/{postId}/comments")]
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await _communityService.GetCommentsByPostIdAsync(postId);
            return Ok(comments);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto dto)
        {
            var result = await _communityService.AddCommentAsync(dto);
            if (!result) return BadRequest(new { message = "Error adding comment" });
            return Ok(new { message = "Comment added" });
        }

        [HttpPost("posts/{postId}/like")]
        public async Task<IActionResult> LikePost(int postId, [FromBody] string userId)
        {
            var result = await _communityService.ToggleLikeAsync(postId, userId);
            return Ok(new { success = result });
        }

        [HttpDelete("posts/{postId}")]
        public async Task<IActionResult> DeletePost(int postId, [FromQuery] string userId)
        {
            var result = await _communityService.DeletePostAsync(postId, userId);

            if (!result)
            {
                return NotFound("Post not found or you're not authorized to delete it.");
            }

            return Ok(new { message = "Post deleted successfully" });
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId, [FromQuery] string userId)
        {
            var result = await _communityService.DeleteCommentAsync(commentId, userId);

            if (!result)
            {
                return NotFound("Comment not found or you're not authorized to delete it.");
            }

            return Ok(new { message = "Comment deleted successfully" });
        }
    }
}
