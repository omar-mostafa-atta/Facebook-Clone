using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.IRepository
{
	public interface IChatRepository
	{
		Task<Message> SaveMessageAsync(string senderId, string receiverId, string content);
		Task DeleteMessageAsync(Guid messageId, Guid currentUserId);
	}
}
