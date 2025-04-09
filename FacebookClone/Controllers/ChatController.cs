using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous] // Allow anonymous access for testing
	public class ChatController : ControllerBase
	{
		private readonly IChatRepository _chatService;

		public ChatController(IChatRepository chatService)
		{
			_chatService = chatService;
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO dto)
		{
			try
			{
				var message = await _chatService.SaveMessageAsync(dto.SenderId, dto.ReceiverId, dto.Content);

				// Map the Message entity to a DTO to avoid circular references
				var messageResponse = new MessageResponseDTO
				{
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
	}
}