using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : BaseController
	{
		private readonly IUserRepository _userRepository;

		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[HttpPost("UploadProfilePicture")]
		[Authorize]
		public async Task<IActionResult> UploadProfilePicture(UploadUserPictureDTO uploadUserPictureDTO)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}
				await _userRepository.UploadProfilePictureAsync(appUserId, uploadUserPictureDTO.formFile);

				return Ok();
			});
		}

		[HttpPost("UploadBackGroundPicture")]
		[Authorize]
		public async Task<IActionResult> UploadBackGroundPicture(UploadUserPictureDTO uploadUserPictureDTO)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}
				await _userRepository.UploadBackgroundPictureAsync(appUserId, uploadUserPictureDTO.formFile);

				return Ok();
			});
		}
		[HttpDelete("DeleteBackGroundPicture")]
		[Authorize]
		public async Task<IActionResult> DeleteBackGroundPicture()
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}
				await _userRepository.DeleteBackgroundPictureAsync(appUserId);

				return Ok();
			});
		}

		[HttpDelete("DeleteProfilePicture")]
		[Authorize]
		public async Task<IActionResult> DeleteProfilePicture()
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(userId, out var appUserId))
				{
					return BadRequest("Invalid user ID.");
				}
				await _userRepository.DeleteProfilePictureAsync(appUserId);

				return Ok();
			});
		}

		[HttpPost("AddBio")]
		[Authorize]
		public async Task<IActionResult> AddBio([FromBody] AddBioDTO addBioDTO)
		{
			return await HandleRequest(async () =>
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				await _userRepository.AddBio(addBioDTO.text, userId);
				return Ok();
			});

		}

		[HttpGet("profile/{userId}")]
		[Authorize]
		public async Task<IActionResult> GetUserProfile(string userId)
		{
			return await HandleRequest(async () =>
			{
				var userProfile = await _userRepository.GetUserProfileAsync(userId);
				return Ok(userProfile);
			});
		}
	}
}
