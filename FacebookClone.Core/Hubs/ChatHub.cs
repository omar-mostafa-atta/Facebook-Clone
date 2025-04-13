using FacebookClone.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace FacebookClone.API.Hubs
{
	public class ChatHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			var userIdStr = Context.GetHttpContext()?.Request.Query["userId"];
			if (Guid.TryParse(userIdStr, out var userId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
				Console.WriteLine($"User {userId} connected with ConnectionId {Context.ConnectionId}");
			}
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var userIdStr = Context.GetHttpContext()?.Request.Query["userId"];
			if (Guid.TryParse(userIdStr, out var userId))
			{
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
				Console.WriteLine($"User {userId} disconnected");
			}
			await base.OnDisconnectedAsync(exception);
		}

	 
		public async Task SendMessage(Guid senderId, Guid receiverId, string content)
		{
			await Clients.Group(receiverId.ToString()).SendAsync("ReceiveMessage", new
			{
				SenderId = senderId,
				Content = content,
				SentAt = DateTime.UtcNow,
			
			});
		}
		public async Task DeleteMessage(Guid messageId)
		{
			 
			await Clients.All.SendAsync("MessageDeleted", new
			{
				messageId = messageId.ToString()
			});
		}
	}
}