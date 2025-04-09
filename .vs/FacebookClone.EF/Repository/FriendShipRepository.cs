using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.EF;
using Microsoft.EntityFrameworkCore;

namespace FacebookClone.Core.Services
{
	public class FriendShipRepository : IFriendShipRepository
	{
		private readonly FacebookContext _context;

		public FriendShipRepository(FacebookContext context)
		{
			_context = context;
		}

		public async Task<Friendship> SendFriendRequestAsync(Guid senderId, Guid receiverId)
		{
			var sender = await _context.Users.FindAsync(senderId);
			var receiver = await _context.Users.FindAsync(receiverId);

			if (sender == null || receiver == null)
			{
				throw new KeyNotFoundException("User not found");
			}

			var existingRequest = await _context.Friendship
				.FirstOrDefaultAsync(f =>
					(f.SenderId == senderId && f.ReciverId == receiverId) ||
					(f.SenderId == receiverId && f.ReciverId == senderId));

			if (existingRequest != null)
			{
				throw new ArgumentException("Friendship request already exists");
			}

			var friendship = new Friendship
			{
				SenderId = senderId,
				ReciverId = receiverId,
				Status = FriendshipStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};

			_context.Friendship.Add(friendship);
			await _context.SaveChangesAsync();

			return friendship;
		}

		public async Task<List<Friendship>> GetPendingRequestsAsync(Guid userId)
		{
			return await _context.Friendship
				.Where(f => f.ReciverId == userId && f.Status == FriendshipStatus.Pending)
				.Include(f => f.Sender)
				.ToListAsync();
		}

		public async Task RemoveFriendRequestAsync(Guid friendshipId, Guid userId)
		{
			var friendship = await _context.Friendship
				.FirstOrDefaultAsync(f => f.Id == friendshipId);

			if (friendship == null)
			{
				throw new KeyNotFoundException("Friend request not found");
			}

		 
			if (friendship.SenderId != userId && friendship.ReciverId != userId)
			{
				throw new UnauthorizedAccessException("You are not authorized to remove this friend request");
			}

			 
			if (friendship.Status != FriendshipStatus.Pending)
			{
				throw new InvalidOperationException("Cannot remove a friend request that is not pending");
			}

			_context.Friendship.Remove(friendship);
			await _context.SaveChangesAsync();
		}
	}
}