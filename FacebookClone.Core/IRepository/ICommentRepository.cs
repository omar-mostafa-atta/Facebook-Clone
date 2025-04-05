using FacebookClone.Core.DTO;
using FacebookClone.Core.Models;


namespace FacebookClone.Core.IRepository
{
	public interface ICommentRepository
	{
		Task Create(AddCommentDTO addCommentDTO, AppUser user);
		Task Delete(string CommentId,AppUser user);
		Task Update(UpdateCommentDTO updateCommentDTO, AppUser user);
		Task<List<Comment>> GetPostComments(string postId);
	}
}
