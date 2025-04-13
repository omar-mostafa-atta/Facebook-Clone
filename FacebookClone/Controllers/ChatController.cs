using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChatController : ControllerBase
	{
		private readonly IChatRepository _chatService;

		public ChatController(IChatRepository chatService)
		{
			_chatService = chatService;
		}

		[HttpPost("send")]
		[Authorize]
		public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO dto)
		{
			try
			{
				var message = await _chatService.SaveMessageAsync(dto.SenderId, dto.ReceiverId, dto.Content);

				var messageResponse = new MessageResponseDTO
				{
					MessageId = message.Id.ToString(),
					ChatId = message.ChatId.ToString(),
					SenderId = message.SenderId.ToString(),
					Content = message.Content,
					SentAt = message.SentAt
				};

				return Ok(messageResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}
		[HttpDelete("delete/{messageId}")]
		[Authorize]
		public async Task<IActionResult> DeleteMessage(string messageId)
		{
			try
			  {
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!Guid.TryParse(messageId, out var parsedMessageId))
				{
					return BadRequest(new { error = "Invalid message ID format" });
				}
				if (!Guid.TryParse(userId, out var userid))
				{
					return BadRequest(new { error = "Invalid message ID format" });
				}
				await _chatService.DeleteMessageAsync(parsedMessageId, parsedMessageId);
				return Ok(new { message = "Message deleted successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}
	}
}