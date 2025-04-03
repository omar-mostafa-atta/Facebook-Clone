using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReactionController : ControllerBase
	{
		private readonly IPostRepository _postService;
		private readonly UserManager<AppUser> _userManager;

		public ReactionController(IPostRepository _postService,  UserManager<AppUser> _userManager)
		{
			this._postService = _postService;
			this._userManager= _userManager;
		}


		[HttpPost("AddReaction")]
		[Authorize]
		public async Task<IActionResult> AddReaction(AddReactionDTO addReactionDTO)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					return Unauthorized();
				await _postService.AddReaction(addReactionDTO, user);
				return Ok("Reaction is Added");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		[HttpPost("RemoveReaction")]
		[Authorize]
		public async Task<IActionResult> RemoveReaction([FromBody] RemoveReactionDTO removeReactionDTO)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					return Unauthorized();

				await _postService.RemoveReaction(removeReactionDTO.PostId, user);
				return Ok("Reaction is Removed");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

	}
}
