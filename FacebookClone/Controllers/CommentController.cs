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
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var user = await _userManager.FindByIdAsync(userId);
				await _commentRepository.Create(addCommentDTO, user);
				return Created();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex);
			}
			catch (ArgumentException ex)
			{

				return BadRequest(ex);

			}
		}
	}
}