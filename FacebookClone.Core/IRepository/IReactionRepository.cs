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
	}
}