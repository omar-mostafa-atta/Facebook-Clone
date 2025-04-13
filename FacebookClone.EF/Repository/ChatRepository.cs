using FacebookClone.API.Hubs;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.EF;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class ChatRepository : IChatRepository
{
	private readonly FacebookContext _context;
	private readonly IHubContext<ChatHub> _hubContext;

	public ChatRepository(FacebookContext context, IHubContext<ChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	}

	public async Task<Message> SaveMessageAsync(string senderid, string receiverid, string content)
	{
		if (!Guid.TryParse(senderid, out var senderId))
		{
			throw new ArgumentException("Wrong Guid format for senderId");
		}

		if (!Guid.TryParse(receiverid, out var receiverId))
		{
			throw new ArgumentException("Wrong Guid format for receiverId");
		}

		var chat = await GetOrCreateChatAsync(senderId, receiverId);

		var message = new Message
		{
			ChatId = chat.Id,
			SenderId = senderId,
			Content = content,
			SentAt = DateTime.UtcNow
		};

		_context.Message.Add(message);
		await _context.SaveChangesAsync();

		await _hubContext.Clients.Group(receiverId.ToString()).SendAsync("ReceiveMessage", new
		{
			senderId,
			content,
			timestamp = message.SentAt
			 
		});

		return message;
	}
	public async Task DeleteMessageAsync(Guid messageId,Guid currentUserId)
	{
		var message = await _context.Message
			.Include(m => m.Chat)
			.FirstOrDefaultAsync(m => m.Id == messageId);

		if (message == null)
		{
			throw new ArgumentException("Message not found");
		}

		if (message.SenderId != currentUserId)
		{
			throw new UnauthorizedAccessException("You can only delete your own messages");
		}

		_context.Message.Remove(message);
		await _context.SaveChangesAsync();

		 
		var senderId = message.SenderId.ToString();
		var receiverId = message.Chat.ReceiverId == message.SenderId
			? message.Chat.SenderId.ToString()
			: message.Chat.ReceiverId.ToString();

		await _hubContext.Clients.Group(senderId).SendAsync("MessageDeleted", new
		{
			messageId = messageId.ToString()
		});

		await _hubContext.Clients.Group(receiverId).SendAsync("MessageDeleted", new
		{
			messageId = messageId.ToString()
		});
	}
	private async Task<Chat> GetOrCreateChatAsync(Guid senderId, Guid receiverId)
	{
		var existingChat = await _context.Chat
			.FirstOrDefaultAsync(c =>
				(c.SenderId == senderId && c.ReceiverId == receiverId) ||
				(c.SenderId == receiverId && c.ReceiverId == senderId));

		if (existingChat != null)
			return existingChat;

		var newChat = new Chat
		{
			SenderId = senderId,
			ReceiverId = receiverId
		};

		_context.Chat.Add(newChat);
		await _context.SaveChangesAsync();
		return newChat;
	}
}