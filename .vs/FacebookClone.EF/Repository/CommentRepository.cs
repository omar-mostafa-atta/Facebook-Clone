using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.EF.Repository
{
	public class CommentRepository:ICommentRepository
	{
		
		private readonly IGenericRepository<Comment> _commentgenericRepository;
		private readonly IGenericRepository<Post> _postgenericRepository;
		

		public CommentRepository( IGenericRepository<Comment> commentgenericRepository, IGenericRepository<Post> postgenericRepository)
		{
			_commentgenericRepository = commentgenericRepository;
			_postgenericRepository = postgenericRepository;
		}

		public async Task Create(AddCommentDTO addCommentDTO,AppUser user)
		{
			if (!Guid.TryParse(addCommentDTO.PostId, out var Post))
			{
				throw new ArgumentException("Wrong Guid format");
			}
			var post =await _postgenericRepository.GetByIdAsync(Post);
			if (post == null)
				throw new KeyNotFoundException("Post not found");
			 
			var comment = new Comment{
				Text = addCommentDTO.Text,
				PostId = Post,
				CreatedAt = DateTime.Now,
				AppUserId = user.Id

			};
			post.TotalComments += 1;
			await _postgenericRepository.Update(post);
			await _commentgenericRepository.AddAsync(comment);
			await _commentgenericRepository.SaveChangesAsync();
		}

		public async Task Update(UpdateCommentDTO updateCommentDTO, AppUser user)
		{

			if (!Guid.TryParse(updateCommentDTO.Id, out var CommentId))
			{
				throw new ArgumentException("Wrong Guid format");
			}

			var comment = await _commentgenericRepository.GetByIdAsync(CommentId);

			if (comment == null)
				throw new KeyNotFoundException("Comment not found");

			else if (comment.AppUserId != user.Id)
				throw new UnauthorizedAccessException();

			comment.UpdatedAt = DateTime.Now;
			comment.Text = updateCommentDTO.Text;
			await _commentgenericRepository.Update(comment);


		}

		public async Task Delete(string CommentId,AppUser user)
		{
			if (!Guid.TryParse(CommentId, out var Comment))
			{
				throw new ArgumentException("Wrong Guid format");
			}

			var comment = await _commentgenericRepository.GetByIdAsync(Comment);
			 
			if(comment == null)
				throw new KeyNotFoundException("Comment not found");
			else if(comment.AppUserId!=user.Id)
				throw new UnauthorizedAccessException();

			var post = await _postgenericRepository.GetByIdAsync(comment.PostId);
			
			if (post == null)
				throw new KeyNotFoundException("Post not found for this comment");

			post.TotalReactions = Math.Max(0, post.TotalComments - 1);

			await _postgenericRepository.Update(post);
			await _commentgenericRepository.Delete(comment);
			await _commentgenericRepository.SaveChangesAsync();
		}

		public async Task<List<Comment>> GetPostComments(string postId)
		{
			if (!Guid.TryParse(postId, out var postid))
			{
				throw new ArgumentException("Wrong Guid format");
			}
			var post=await _postgenericRepository.GetByIdAsync(postid);
			
			if (post == null)
				throw new KeyNotFoundException("Post not found");

			var postComments = post.Comments.ToList();

			return postComments;

		}
	}
}
