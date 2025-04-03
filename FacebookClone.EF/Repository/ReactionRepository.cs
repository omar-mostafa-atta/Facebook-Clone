using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.Core.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
 

namespace FacebookClone.EF.Repository
{
	public class ReactionRepository:IReactionRepository
	{
		private readonly FacebookContext _context;
	 
		
		private readonly IGenericRepository<Post> _postgenericRepository;
		private readonly IGenericRepository<Reactions> _reactionsgenericRepository;
		 

		public ReactionRepository(FacebookContext context, IGenericRepository<Reactions> _reactionsgenericRepository, IGenericRepository<Post> _postgenericRepository )
		{
			_context = context;
			this._postgenericRepository = _postgenericRepository;
			this._reactionsgenericRepository = _reactionsgenericRepository;
		}
		public async Task<Reactions> GetReactionByUserAndPostAsync(Guid userId, Guid postId)
		{
			return await _context.Reactions
				.FirstOrDefaultAsync(r => r.AppUserId == userId && r.PostId == postId);
		}
		public async Task<List<Reactions>> GetReactionsByPostId(string postId)
		{
			return await _context.Reactions.AsNoTracking().Where(r => r.PostId == Guid.Parse(postId)).ToListAsync();
		}

		public async Task<List<GetReactionsDTO>> GetReaction(string postId)
		{
			var Post=await _postgenericRepository.GetByIdAsync(postId);
			if (Post == null)
				throw new KeyNotFoundException("No Post for this Id");

			var reactions = await GetReactionsByPostId(postId);

			var reactionDtos = reactions.Select(r => new GetReactionsDTO
			{
				reactionType = r.ReactionType,
				userId = r.AppUserId
			}).ToList();

			return reactionDtos;
		}


		public async Task AddReaction(AddReactionDTO addReactionDTO, AppUser user)
		{
			if (!Guid.TryParse(addReactionDTO.PostId, out var parsedPostId))
			{
				throw new Exception("Wrong GUID post format");
			}

			var existingPost = await _postgenericRepository.GetByIdAsync(addReactionDTO.PostId);
			if (existingPost == null)
			{
				throw new KeyNotFoundException("Post not found");
			}

			if (!TryParseReactionType(addReactionDTO.ReactionType, out var reactionType))
			{
				throw new ArgumentException("Invalid reaction type provided");
			}
			var existingReaction = await GetReactionByUserAndPostAsync(user.Id, parsedPostId);
			if (existingReaction != null)
			{

				existingReaction.ReactionType = reactionType;
				await _reactionsgenericRepository.Update(existingReaction);

				return;
			}
			var Reaction = new Reactions
			{
				AppUserId = user.Id,
				ReactionType = reactionType,
				PostId = Guid.Parse(addReactionDTO.PostId)
			};
			existingPost.TotalReactions += 1;
			await _postgenericRepository.Update(existingPost);
			await _reactionsgenericRepository.AddAsync(Reaction);
			await _reactionsgenericRepository.SaveChangesAsync();

		}

		private bool TryParseReactionType(string reactionTypeString, out ReactionType reactionType)
		{
			reactionType = ReactionType.like;
			if (string.IsNullOrWhiteSpace(reactionTypeString))
				return false;


			switch (reactionTypeString.ToLower())
			{
				case "like":
					reactionType = ReactionType.like;
					return true;
				case "love":
					reactionType = ReactionType.love;
					return true;
				case "angry":
					reactionType = ReactionType.angry;
					return true;
				case "sad":
					reactionType = ReactionType.sad;
					return true;
				default:
					return false;
			}
		}

		public async Task RemoveReaction(string PostId, AppUser user)
		{

			if (!Guid.TryParse(PostId, out var parsedPostId))
			{
				throw new ArgumentException("Wrong GUID post format");
			}


			var existingPost = await _postgenericRepository.GetByIdAsync(PostId);
			if (existingPost == null)
			{
				throw new KeyNotFoundException("Post not found");
			}

			var existingReaction = await  GetReactionByUserAndPostAsync(user.Id, parsedPostId);
			if (existingReaction == null)
			{
				throw new KeyNotFoundException("No reaction found for this user on this post");
			}


			existingPost.TotalReactions = Math.Max(0, existingPost.TotalReactions - 1);

			await _postgenericRepository.Update(existingPost);
			_reactionsgenericRepository.Delete(existingReaction);
			await _reactionsgenericRepository.SaveChangesAsync();

		}
	}
}
