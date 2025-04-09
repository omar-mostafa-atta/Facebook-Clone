using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.IRepository
{
	public interface IFriendShipRepository
	{

		Task<Friendship> SendFriendRequestAsync(Guid senderId, Guid receiverId);
		Task<List<Friendship>> GetPendingRequestsAsync(Guid userId);
		Task RemoveFriendRequestAsync(Guid friendshipId, Guid userId);
	}
}
