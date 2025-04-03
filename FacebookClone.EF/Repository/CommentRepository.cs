using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
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
			if (!Guid.TryParse(addCommentDTO.PostId, out var Postt))
			{
				throw new ArgumentException("Wrong Guid format");
			}
			var post =await _postgenericRepository.GetByIdAsync( addCommentDTO.PostId);
			if (post == null)
				throw new KeyNotFoundException("Post not found");
			 

			var comment = new Comment{
				Text = addCommentDTO.Text,
				PostId = Guid.Parse(addCommentDTO.PostId),
				CreatedAt = DateTime.Now,
				AppUserId = user.Id

			};
			post.Comments.Add(comment);
			await _postgenericRepository.Update(post);
			await _commentgenericRepository.AddAsync(comment);
		}
	}
}
