using FacebookClone.Controllers;
using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookClone.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class FriendshipController : BaseController
	{
		private readonly IFriendShipRepository _friendshipService;

		public FriendshipController(IFriendShipRepository friendshipService)
		{
			_friendshipService = friendshipService;
		}

		[HttpPost("sendRequest")]
		[Authorize]
		public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDTO request)
		{
			return await HandleRequest(async () =>
			{
				var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
				var friendship = await _friendshipService.SendFriendRequestAsync(senderId, request.ReceiverId);

				return Ok(new
				{
					Message = "Friend request sent successfully",
					FriendshipId = friendship.Id,
					Status = friendship.Status
				});
			});
		}

		[HttpGet("pendingRequests")]
		[Authorize]
		public async Task<IActionResult> GetPendingRequests()
		{
			return await HandleRequest(async () =>
			{
				var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
				var pendingRequests = await _friendshipService.GetPendingRequestsAsync(userId);

				return Ok(pendingRequests.Select(f => new
				{
					FriendshipId = f.Id,
					SenderId = f.SenderId,
					SenderUsername = f.Sender.UserName,
					CreatedAt = f.CreatedAt
				}));
			});
		}

		[HttpDelete("RemoveRequest/{friendshipId}")]
		[Authorize]
		public async Task<IActionResult> RemoveFriendRequest(string friendshipId)
		{
			return await HandleRequest(async () =>
			{
				if (!Guid.TryParse(friendshipId, out var parsedFriendshipId))
				{
					throw new ArgumentException("Invalid GUID format for Friendship ID.");
				}

				var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
				await _friendshipService.RemoveFriendRequestAsync(parsedFriendshipId, userId);

				return Ok("Friend request removed successfully.");
			});
		}

	
	}

	 
}