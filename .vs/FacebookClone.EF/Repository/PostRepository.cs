﻿using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace FacebookClone.Core.Services
{
	public class PostRepository : IPostRepository
	{
		private readonly IGenericRepository<Post> _postRepository;
		private readonly IMediaRepository _mediaService;
		private readonly IGenericRepository<SavedPosts> _savedPostsRepository;
		private readonly IHubContext<PostHub> _hubContext;
		public PostRepository( IGenericRepository<Post> postRepository, 
			IMediaRepository mediaService, 
			IGenericRepository<SavedPosts> savedPostsRepository,
			IHubContext<PostHub> hubContext)
		{
			_postRepository = postRepository;
			_mediaService = mediaService;
			_savedPostsRepository = savedPostsRepository;
			_hubContext = hubContext;
			
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


			var postDto = new PostDTO
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

		 
			await _hubContext.Clients.All.SendAsync("ReceivePost", postDto);

			return postDto;
		}

	 

		public async Task<PostDTO> UpdatePostAsync(string postId, CreateAndUpdatePostDTO updatePostDto, Guid userId)
		{
			
			if (!Guid.TryParse(postId, out var parsedPostId))
			{
				throw new ArgumentException("Invalid GUID format for Post ID.");
			}

		
			var post = await _postRepository.GetByIdAsync(parsedPostId);
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

			
			var post = await _postRepository.GetByIdAsync(postId);
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
			
			var post = await _postRepository.GetByIdAsync(postId);
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

		public async Task DeletetAsync(Guid postId, Guid userId)
		{
			var post = await _postRepository.GetByIdAsync(postId);
			if (post == null)
			{ throw new KeyNotFoundException("Post not found"); }
			if (post.AppUserId != userId)
			{
				throw new UnauthorizedAccessException();

			}
			await _postRepository.Delete(post);
			await _postRepository.SaveChangesAsync();

		}

	}
}
