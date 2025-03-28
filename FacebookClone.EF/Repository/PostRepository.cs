using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace FacebookClone.Core.Services
{
	public class PostRepository : IPostRepository
	{
		private readonly IGenericRepository<Post> _postRepository;
		private readonly IGenericRepository<Reactions> _ReactionRepository;
		private readonly IReactionRepository _reactionRepository;
		private readonly IMediaRepository _mediaService;
		private readonly IGenericRepository<SavedPosts> _savedPostsRepository;
		public PostRepository(IGenericRepository<Reactions> ReactionRepository, IGenericRepository<Post> postRepository, 
			IMediaRepository mediaService, 
			IGenericRepository<SavedPosts> savedPostsRepository, 
			IReactionRepository reactionRepository)
		{
			_postRepository = postRepository;
			_mediaService = mediaService;
			_savedPostsRepository = savedPostsRepository;
			
			_ReactionRepository=ReactionRepository;
			_reactionRepository = reactionRepository;
		}

		public async Task<PostDTO> CreatePostAsync(CreateAndUpdatePostDTO createPostDto, Guid userId)
		{
			
			if (string.IsNullOrWhiteSpace(createPostDto.Text) &&
				(createPostDto.MediaFiles == null || createPostDto.MediaFiles.Count == 0))
			{
				throw new ArgumentException("Post must have either text or media.");
			}

			
			var post = new Post
			{
				Text = createPostDto.Text,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				AppUserId = userId
			};

			await _postRepository.AddAsync(post);
			await _postRepository.SaveChangesAsync();

			
			if (createPostDto.MediaFiles != null && createPostDto.MediaFiles.Count > 0)
			{
				foreach (var file in createPostDto.MediaFiles)
				{
					if (file.Length > 0)
					{
						var media = await _mediaService.UploadMediaAsync(file, post.Id);
						post.Media.Add(media);
					}
				}
				await _postRepository.Update(post);
				 
			}

			
			return new PostDTO
			{
				Id = post.Id,
				Text = post.Text,
				CreatedAt = post.CreatedAt,
				UpdatedAt = post.UpdatedAt,
				TotalReactions = post.TotalReactions,
				AppUserId = post.AppUserId,
				Media = post.Media.Select(m => new MediaDto
				{
					Id = m.Id,
					Type = m.Type,
					Url = m.Url,
					PublicId = m.PublicId,
					PostId = m.PostId
				}).ToList()
			};
		}

	 

		public async Task<PostDTO> UpdatePostAsync(string postId, CreateAndUpdatePostDTO updatePostDto, Guid userId)
		{
			
			if (!Guid.TryParse(postId, out var parsedPostId))
			{
				throw new ArgumentException("Invalid GUID format for Post ID.");
			}

		
			var post = await _postRepository.GetByIdAsync(postId);
			if (post == null)
			{
				throw new KeyNotFoundException("Post not found.");
			}

	
			if (post.AppUserId != userId)
			{
				throw new UnauthorizedAccessException("You do not have permession to update this post.");
			}

			
			if (string.IsNullOrWhiteSpace(updatePostDto.Text) &&(updatePostDto.MediaFiles == null || updatePostDto.MediaFiles.Count == 0))
			{
				throw new ArgumentException("Post must have either text or media.");
			}

			if (!string.IsNullOrWhiteSpace(updatePostDto.Text))
			{
				post.Text = updatePostDto.Text;
			}
			post.UpdatedAt = DateTime.UtcNow;

			
			if (updatePostDto.MediaFiles != null && updatePostDto.MediaFiles.Count > 0)
			{
				foreach (var file in updatePostDto.MediaFiles)
				{
					if (file.Length > 0)
					{
						var media = await _mediaService.UploadMediaAsync(file, post.Id);
						post.Media.Add(media);
					}
				}
			}

			
			await _postRepository.Update(post);
			 

			
			return new PostDTO
			{
				Id = post.Id,
				Text = post.Text,
				CreatedAt = post.CreatedAt,
				UpdatedAt = post.UpdatedAt,
				TotalReactions = post.TotalReactions,
				AppUserId = post.AppUserId,
				Media = post.Media.Select(m => new MediaDto
				{
					Id = m.Id,
					Type = m.Type,
					Url = m.Url,
					PublicId = m.PublicId,
					PostId = m.PostId
				}).ToList()
			};
		}

		public async Task SavePostAsync(Guid postId, Guid userId)
		{
			
			var post = await _postRepository.GetByIdAsync(postId.ToString());
			if (post == null)
			{
				throw new KeyNotFoundException("Post not found.");
			}

			
			var existingSavedPost = (await _savedPostsRepository.FindAsync(sp => sp.AppUserId == userId && sp.PostId == postId)).FirstOrDefault();
			if (existingSavedPost != null)
			{
				throw new InvalidOperationException("Post is already saved by this user.");
			}

			var savedPost = new SavedPosts
			{
				AppUserId = userId, 
				PostId = postId,
				SavedAt = DateTime.UtcNow
			};

			await _savedPostsRepository.AddAsync(savedPost);
			await _savedPostsRepository.SaveChangesAsync();
		}

		public async Task UnsavePostAsync(Guid postId, Guid userId)
		{
			
			var post = await _postRepository.GetByIdAsync(postId.ToString());
			if (post == null)
			{
				throw new KeyNotFoundException("Post not found.");
			}

			
			var existingSavedPost = (await _savedPostsRepository.FindAsync(sp => sp.AppUserId == userId && sp.PostId == postId)).FirstOrDefault();
			if (existingSavedPost == null)
			{
				throw new InvalidOperationException("Post is not saved by this user.");
			}

			
			 _savedPostsRepository.Delete(existingSavedPost);
			await _savedPostsRepository.SaveChangesAsync();
		}

		public async Task AddReaction(AddReactionDTO addReactionDTO, AppUser user)
		{
			if (!Guid.TryParse(addReactionDTO.PostId, out var parsedPostId))
			{
				throw new Exception("Wrong GUID post format");
			}
			
			var existingPost = await _postRepository.GetByIdAsync(addReactionDTO.PostId);
			if (existingPost == null)
			{
				throw new KeyNotFoundException("Post not found");
			}

			if (!TryParseReactionType(addReactionDTO.ReactionType, out var reactionType))
			{
				throw new ArgumentException("Invalid reaction type provided");
			}
			var existingReaction = await _reactionRepository.GetReactionByUserAndPostAsync(user.Id, parsedPostId);
			if (existingReaction != null)
			{
				
				existingReaction.ReactionType = reactionType;
				await _ReactionRepository.Update(existingReaction);
			
				return; 
			}
			var Reaction = new Reactions
			{
				AppUserId = user.Id,
				ReactionType = reactionType,
				PostId = Guid.Parse(addReactionDTO.PostId)
			};
			existingPost.TotalReactions += 1;
			 await _postRepository.Update(existingPost);
			await _ReactionRepository.AddAsync(Reaction);
			await _ReactionRepository.SaveChangesAsync();

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
			
			if (!Guid.TryParse( PostId, out var parsedPostId))
			{
				throw new ArgumentException("Wrong GUID post format");
			}

			
			var existingPost = await _postRepository.GetByIdAsync(PostId);
			if (existingPost == null)
			{
				throw new KeyNotFoundException("Post not found");
			}

			var existingReaction = await _reactionRepository.GetReactionByUserAndPostAsync(user.Id, parsedPostId);
			if (existingReaction == null)
			{
				throw new KeyNotFoundException("No reaction found for this user on this post");
			}

		
			existingPost.TotalReactions -= 1;
			if (existingPost.TotalReactions < 0) 
			{
				existingPost.TotalReactions = 0;
			}

		
		    _ReactionRepository.Delete(existingReaction);
			await _postRepository.Update(existingPost);
			await _ReactionRepository.SaveChangesAsync();
		}

	}
}
