using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.EF.Repository
{
	public class ReactionRepository:IReactionRepository
	{
		private readonly FacebookContext _context;

		public ReactionRepository(FacebookContext context)
		{
			_context = context;
		}
		public async Task<Reactions> GetReactionByUserAndPostAsync(Guid userId, Guid postId)
		{
			return await _context.Reactions
				.FirstOrDefaultAsync(r => r.AppUserId == userId && r.PostId == postId);
		}
	}
}
