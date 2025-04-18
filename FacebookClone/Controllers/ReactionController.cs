using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReactionController : BaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IReactionRepository _reactionRepository;

		public ReactionController(UserManager<AppUser> userManager, IReactionRepository reactionRepository)
		{
			_userManager = userManager;
			_reactionRepository = reactionRepository;
		}

		[HttpPost("AddReaction")]
		[Authorize]
		public async Task<IActionResult> AddReaction([FromBody] AddReactionDTO addReactionDTO)
		{
			return await HandleRequest(async () =>
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					throw new KeyNotFoundException("User not found");

				await _reactionRepository.AddReaction(addReactionDTO, user);
				return Ok("Reaction is Added");
			});
		}

		[HttpPost("RemoveReaction")]
		[Authorize]
		public async Task<IActionResult> RemoveReaction([FromBody] RemoveReactionDTO removeReactionDTO)
		{
			return await HandleRequest(async () =>
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
					throw new KeyNotFoundException("User not found");

				await _reactionRepository.RemoveReaction(removeReactionDTO.PostId, user);
				return Ok("Reaction is Removed");
			});
		}

		[HttpGet("GetReactions/{postId}")]
		public async Task<IActionResult> GetReaction([FromRoute] string postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			return await HandleRequest(async () =>
			{
				if (string.IsNullOrEmpty(postId) || !Guid.TryParse(postId, out _))
				{
					throw new ArgumentException("Invalid Post ID format.");
				}

				var reactions = await _reactionRepository.GetReaction(postId, pageNumber, pageSize);
				return Ok(reactions);
			});
		}

		[HttpGet("GetReactionCounts/{postId}")]
		[Authorize]
		public async Task<IActionResult> GetReactionCounts(string postId)
		{
			return await HandleRequest(async () =>
			{
				if (string.IsNullOrEmpty(postId) || !Guid.TryParse(postId, out _))
				{
					throw new ArgumentException("Invalid Post ID format.");
				}

				var reactionCounts = await _reactionRepository.GetReactionCountByPostId(postId);
				return Ok(reactionCounts);
			});
		}

	 
	}
}