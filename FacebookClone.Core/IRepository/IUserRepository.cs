using FacebookClone.Core.DTO;
using Microsoft.AspNetCore.Http;
  
namespace FacebookClone.Core.IRepository
{
	public interface IUserRepository
	{
		Task UploadProfilePictureAsync(Guid userId, IFormFile file);
		Task UploadBackgroundPictureAsync(Guid userId, IFormFile file);
		Task DeleteProfilePictureAsync(Guid userId);
		Task DeleteBackgroundPictureAsync(Guid userId);
		Task<UserProfileDto> GetUserProfileAsync(string userId);

		Task AddBio(string bio,string userId);
	}
}
