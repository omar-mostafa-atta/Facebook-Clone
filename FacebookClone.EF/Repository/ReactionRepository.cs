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

		public async Task<List<GetReactionsDTO>> GetReaction(string postId, int pageNumber = 1, int pageSize = 10)
		{
			if (!Guid.TryParse(postId, out var parsedPostId))
			{
				throw new ArgumentException("Invalid GUID format for Post ID.");
			}

			var post = await _postgenericRepository.GetByIdAsync(parsedPostId);
			if (post == null)
				throw new KeyNotFoundException("No Post for this Id");

			var reactions = await GetReactionsByPostId(postId);

			var pagedReactions = reactions
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var reactionDtos = pagedReactions.Select(r => new GetReactionsDTO
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

			var existingPost = await _postgenericRepository.GetByIdAsync(parsedPostId);
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


			var existingPost = await _postgenericRepository.GetByIdAsync(parsedPostId);
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
			await _reactionsgenericRepository.Delete(existingReaction);
			await _reactionsgenericRepository.SaveChangesAsync();

		}

		public async Task<ReactionCountDTO> GetReactionCountByPostId(string postId)
		{
			if (!Guid.TryParse(postId, out var parsedPostId))
			{
				throw new ArgumentException("Invalid GUID format for Post ID.");
			}

			var grouped = await _context.Reactions
				.Where(r => r.PostId == parsedPostId)
				.GroupBy(r => r.ReactionType)
				.Select(g => new { Type = g.Key, Count = g.Count() })
				.ToListAsync();

			var dto = new ReactionCountDTO();

			foreach (var g in grouped)
			{
				switch (g.Type)
				{
					case ReactionType.like:
						dto.Likes = g.Count;
						break;
					case ReactionType.love:
						dto.Loves = g.Count;
						break;
					case ReactionType.angry:
						dto.Angries = g.Count;
						break;
					case ReactionType.sad:
						dto.Sads = g.Count;
						break;
				}
			}

			return dto;
		}

	}
}
