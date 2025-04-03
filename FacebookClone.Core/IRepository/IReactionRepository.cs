using FacebookClone.Core.DTO;
using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.IRepository
{
	public interface IReactionRepository
	{
		Task<Reactions> GetReactionByUserAndPostAsync(Guid userId, Guid postId);
		Task<List<Reactions>> GetReactionsByPostId(string postId);
		Task AddReaction(AddReactionDTO addReactionDTO, AppUser user);
		Task RemoveReaction(string PostId, AppUser user);
		Task<List<GetReactionsDTO>> GetReaction(string postId);
	}
}