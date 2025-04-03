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
		
		private readonly UserManager<AppUser> _userManager;
		private readonly IReactionRepository _reactionRepository;
		public ReactionController(  UserManager<AppUser> _userManager, IReactionRepository _reactionRepository)
		{
			
			this._userManager= _userManager;
			this._reactionRepository = _reactionRepository;
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
				await _reactionRepository.AddReaction(addReactionDTO, user);
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

				await _reactionRepository.RemoveReaction(removeReactionDTO.PostId, user);
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
		[HttpGet("GetPostReactions")]
		public async Task<IActionResult> GetReaction([FromQuery]string postId)
		{
			try
			{
				var reactions = await _reactionRepository.GetReaction(postId);
				return Ok(reactions);
			}
			catch (KeyNotFoundException ex)
			{

				return NotFound(ex.Message);
			}
		}

	}
}
