using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentController : ControllerBase
	{
		private readonly IPostRepository _postService;
		private readonly UserManager<AppUser> _userManager;
		private readonly ICommentRepository _commentRepository;

		public CommentController(IPostRepository _postService, UserManager<AppUser> _userManager, ICommentRepository commentRepository)
		{
			this._postService = _postService;
			this._userManager = _userManager;
			_commentRepository = commentRepository;
		}

		[HttpPost("Add")]
		[Authorize]
		public async Task<IActionResult> Add(AddCommentDTO addCommentDTO)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var user = await _userManager.FindByIdAsync(userId);
				await _commentRepository.Create(addCommentDTO, user);
				return Created();
			});
		}

		[HttpPatch("Edit")]
		[Authorize]
		public async Task<IActionResult> Edit([FromBody]UpdateCommentDTO updateCommentDTO)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null) return Unauthorized("User not found.");

				await _commentRepository.Update(updateCommentDTO, user);
				return Ok("Comment updated successfully.");
			});
		}


		[HttpDelete("Delete")]
		[Authorize]
		public async Task<IActionResult> Delete(string commentId)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null) return Unauthorized("User not found.");

				await _commentRepository.Delete(commentId, user);
				return Ok("Comment Deleted successfully.");
			});
		}

		[HttpGet("GetPostComments/{postid}")]
		[Authorize]
		public async Task<IActionResult> GetPostComments(string postid)
		{
			return await HandleRequest(async () =>
			{
				var comments=await _commentRepository.GetPostComments(postid);
				return Ok(comments);
			});
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