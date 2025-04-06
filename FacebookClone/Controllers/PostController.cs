using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
		private readonly UserManager<AppUser> _userManager;

		public PostController(
			IGenericRepository<Post> postrepository,
			IMediaRepository mediaService,
			IPostRepository postService,
			UserManager<AppUser> userManager)
		{
			_postrepository = postrepository;
			_mediaService = mediaService;
			_postService = postService;
			_userManager = userManager;
		}

		[HttpGet("GetAllPosts")]
		public async Task<ActionResult<List<PostDTO>>> GetAllPosts()
		{
			return await HandleRequest<List<PostDTO>>(async () =>
			{
				var posts = await _postrepository.GetAllAsync();

				if (posts == null || !posts.Any())
					throw new KeyNotFoundException("No posts found.");

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
			});
		}

		[HttpGet("Get/{id}")]
		public async Task<ActionResult<PostDTO>> GetPost(string id)
		{
			return await HandleRequest<PostDTO>(async () =>
			{
				if (!Guid.TryParse(id, out var guidId))
				{
					throw new ArgumentException("Invalid GUID format for Post ID.");
				}

				var post = await _postrepository.GetByIdAsync(guidId);
				if (post == null)
					throw new KeyNotFoundException("Post not found.");

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
			});
		}

		[HttpGet("GetUserPosts/{id}")]
		public async Task<ActionResult<List<PostDTO>>> GetUserPosts(string id)
		{
			return await HandleRequest<List<PostDTO>>(async () =>
			{
				if (!Guid.TryParse(id, out var guidId))
				{
					throw new ArgumentException("Invalid GUID format for User ID.");
				}

				var posts = await _postrepository.FindAsync(p => p.AppUserId == guidId);

				if (posts == null || !posts.Any())
					throw new KeyNotFoundException("This user has no posts.");

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
			});
		}

		[HttpPost("Create")]
		[Authorize]
		public async Task<ActionResult<PostDTO>> Create([FromForm] CreateAndUpdatePostDTO createPostDto)
		{
			return await HandleRequest<PostDTO>(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					throw new ArgumentException("Invalid user ID.");
				}

				var postDto = await _postService.CreatePostAsync(createPostDto, appUserId);
				return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
			});
		}

		[HttpPut("Update/{id}")]
		[Authorize]
		public async Task<IActionResult> Update(string id, [FromForm] CreateAndUpdatePostDTO updatePostDto)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					throw new ArgumentException("Invalid user ID.");
				}

				var postDto = await _postService.UpdatePostAsync(id, updatePostDto, appUserId);
				return Ok(postDto);
			});
		}

		[HttpPost("Save")]
		[Authorize]
		public async Task<IActionResult> Save([FromBody] SavePostDTO savePostDTO)
		{
			return await HandleRequest(async () =>
			{
				if (!Guid.TryParse(savePostDTO.PostId, out var parsedPostId))
				{
					throw new ArgumentException("Invalid GUID format for Post ID.");
				}

				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userIdClaim, out var appUserId))
				{
					throw new ArgumentException("Invalid user ID.");
				}

				await _postService.SavePostAsync(parsedPostId, appUserId);
				return Ok("Post saved successfully.");
			});
		}

		[HttpPost("UnsavePost")]
		[Authorize]
		public async Task<IActionResult> UnsavePost([FromBody] UnsavePostDTO unsavePostDTO)
		{
			return await HandleRequest(async () =>
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					throw new UnauthorizedAccessException("User not authenticated.");

				if (!Guid.TryParse(unsavePostDTO.PostId, out var postId))
				{
					throw new ArgumentException("Invalid Post ID format.");
				}

				await _postService.UnsavePostAsync(postId, user.Id);
				return Ok("Post has been unsaved.");
			});
		}

		[HttpDelete("Delete/{PostId}")]
		[Authorize]
		public async Task<IActionResult> Delete([FromQuery] string PostId)
		{
			return await HandleRequest(async () =>
			{
				if (!Guid.TryParse(PostId, out var parsedPostId))
				{
					throw new ArgumentException("Invalid GUID format for Post ID.");
				}

				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
					throw new UnauthorizedAccessException("User not authenticated.");

				await _postService.DeletetAsync(parsedPostId, Guid.Parse(userId));
				return Ok();
			});
		}

		private async Task<ActionResult<T>> HandleRequest<T>(Func<Task<ActionResult<T>>> action)
		{
			try
			{
				return await action();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An unexpected error occurred: " + ex.Message);
			}
		}

		private async Task<IActionResult> HandleRequest(Func<Task<IActionResult>> action)
		{
			try
			{
				return await action();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An unexpected error occurred: " + ex.Message);
			}
		}
	}
}