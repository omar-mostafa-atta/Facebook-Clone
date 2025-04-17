using FacebookClone.Core.DTO;
 

namespace FacebookClone.Core.IRepository
{
	public interface IPostRepository
	{
		Task<PostDTO> CreatePostAsync(CreateAndUpdatePostDTO createPostDto, Guid userId);
		Task<PostDTO> UpdatePostAsync(string postId, CreateAndUpdatePostDTO updatePostDto, Guid userId);
		Task<List<PostDTO>> GetUserPosts(string id ,int pageNumber = 1, int pageSize = 10, string sortBy = "CreatedAt", string sortDirection = "desc");
		Task SavePostAsync(Guid postId, Guid userId);
		Task UnsavePostAsync(Guid postId, Guid userId);
		Task DeletetAsync(Guid postId, Guid userId);
		Task<PostDTO> SharePostAsync(SharePostDTO sharePostDto, Guid userId);

	}
}
