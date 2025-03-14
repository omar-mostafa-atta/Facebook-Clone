using FacebookClone.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.IRepository
{
	public interface IPostRepository
	{
		Task<PostDTO> CreatePostAsync(CreateAndUpdatePostDTO createPostDto, Guid userId);
		Task<PostDTO> UpdatePostAsync(string postId, CreateAndUpdatePostDTO updatePostDto, Guid userId);
		Task SavePostAsync(Guid postId, Guid userId);
	}
}
