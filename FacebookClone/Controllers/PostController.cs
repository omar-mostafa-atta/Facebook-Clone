using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<AppUser> _userManager;

		public PostController(IGenericRepository<Post> postrepository, IMediaRepository mediaService, IPostRepository postService, UserManager<AppUser> userManager)
		{
			_postrepository = postrepository;
			_mediaService = mediaService;
			_postService = postService;
			_userManager = userManager;
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

		[HttpGet("Get/{id}")]
		public async Task<IActionResult> GetPost(string id)
		{
			if (!Guid.TryParse(id, out var guidId))
			{
				return BadRequest("Invalid GUID format for Post ID.");
			}

			var post = await _postrepository.GetByIdAsync(guidId);
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
		[HttpPost("Create")]

		public async Task<ActionResult<PostDTO>> Create([FromForm] CreateAndUpdatePostDTO createPostDto)
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

		[HttpPut("Update/{id}")]

		public async Task<IActionResult> Update(string id, [FromForm] CreateAndUpdatePostDTO updatePostDto)
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

		[HttpPost("Save")]
		[Authorize]
		public async Task<IActionResult> Save([FromBody] SavePostDTO savePostDTO)
		{
			try
			{
				if (!Guid.TryParse(savePostDTO.PostId, out var parsedPostId))
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
				return Conflict(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		
		[HttpPost("UnsavePost")]
		[Authorize]
		public async Task<IActionResult> UnsavePost([FromBody] UnsavePostDTO unsavePostDTO)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					return Unauthorized();

				if (!Guid.TryParse(unsavePostDTO.PostId, out var postId))
				{
					return BadRequest("Invalid Post ID format.");
				}

				await _postService.UnsavePostAsync(postId, user.Id);
				return Ok("Post has been unsaved.");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		[HttpDelete("Delete/{PostId}")]
		[Authorize]
		public async Task<IActionResult> Delete([FromQuery] string PostId)
		{
			try
			{
				if (!Guid.TryParse(PostId, out var parsedPostId))
				{
					return BadRequest("Invalid GUID format for Post ID.");
				}


				var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


				await _postService.DeletetAsync(parsedPostId, Guid.Parse(userId));
				return Ok();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex);
			}catch(UnauthorizedAccessException ex)
			{
				return Unauthorized();
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An unexpected error occurred: " + ex.Message);
			}
		}

	}


	
}