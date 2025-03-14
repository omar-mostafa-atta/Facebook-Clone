using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly IGenericRepository<Post> _postrepository;
		private readonly IMediaRepository _mediaService;
		private readonly IPostRepository _postService;

		public PostController(IGenericRepository<Post> postrepository, IMediaRepository mediaService, IPostRepository postService)
		{
			_postrepository = postrepository;
			_mediaService = mediaService;
			_postService = postService;
		}

		[HttpGet("GetAllPosts")]
		public async Task<IActionResult> GetAllPosts()
		{
			var posts = await _postrepository.GetAllAsync();

			if (posts == null || !posts.Any())
				return NotFound();

			var postDtos = posts.Select(post => new PostDTO
			{
				Id = post.Id,
				Text = post.Text,
				CreatedAt = post.CreatedAt,
				UpdatedAt = post.UpdatedAt,
				TotalReactions = post.TotalReactions,
				AppUserId = post.AppUserId,
				Media = post.Media.Select(media => new MediaDto
				{
					Id = media.Id,
					Type = media.Type,
					Url = media.Url,
					PublicId = media.PublicId,
					PostId = media.PostId
				}).ToList()
			}).ToList();

			return Ok(postDtos);
		}

		[HttpGet("GetPost/{id}")]
		public async Task<IActionResult> GetPost(string id)
		{
			if (!Guid.TryParse(id, out var guidId))
			{
				return BadRequest("Invalid GUID format for Post ID.");
			}

			var post = await _postrepository.GetByIdAsync(id);
			if (post == null) return NotFound();

			var postDto = new PostDTO
			{
				Id = post.Id,
				Text = post.Text,
				CreatedAt = post.CreatedAt,
				UpdatedAt = post.UpdatedAt,
				TotalReactions = post.TotalReactions,
				AppUserId = post.AppUserId,
				Media = post.Media.Select(media => new MediaDto
				{
					Id = media.Id,
					Type = media.Type,
					Url = media.Url,
					PublicId = media.PublicId,
					PostId = media.PostId
				}).ToList()
			};

			return Ok(postDto);
		}

		[HttpGet("GetUserPosts/{id}")]
		public async Task<IActionResult> GetUserPosts(string id)
		{
			if (!Guid.TryParse(id, out var guidId))
			{
				return BadRequest("Invalid GUID format for User ID.");
			}

			var posts = await _postrepository.FindAsync(p => p.AppUserId == guidId);

			if (posts == null || !posts.Any())
				return NotFound("This user has no Posts");

			var postDtos = posts.Select(post => new PostDTO
			{
				Id = post.Id,
				Text = post.Text,
				CreatedAt = post.CreatedAt,
				UpdatedAt = post.UpdatedAt,
				TotalReactions = post.TotalReactions,
				AppUserId = post.AppUserId,
				Media = post.Media.Select(media => new MediaDto
				{
					Id = media.Id,
					Type = media.Type,
					Url = media.Url,
					PublicId = media.PublicId,
					PostId = media.PostId
				}).ToList()
			}).ToList();

			return Ok(postDtos);
		}
		[HttpPost("CreatePost")]
		[Authorize] 
		public async Task<ActionResult<PostDTO>> CreatePost([FromForm] CreateAndUpdatePostDTO createPostDto)
		{
			try
			{
				
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}

				var postDto = await _postService.CreatePostAsync(createPostDto, appUserId);
				return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
			}

			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		[HttpPut("UpdatePost/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdatePost(string id, [FromForm] CreateAndUpdatePostDTO updatePostDto)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}

				var postDto = await _postService.UpdatePostAsync(id, updatePostDto, appUserId);
				return Ok(postDto);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		[HttpPost("SavePost/{PostId)")]
		[Authorize]
		public async Task<IActionResult> SavePost(string PostId)
		{
			try
			{
				if (!Guid.TryParse(PostId, out var parsedPostId))
				{
					return BadRequest("Invalid GUID format for Post ID.");
				}

				 
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userIdClaim, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}

				 
				await _postService.SavePostAsync(parsedPostId, appUserId);
				return Ok("Post saved successfully.");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(ex.Message); // 409 Conflict for already saved
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

	}
}