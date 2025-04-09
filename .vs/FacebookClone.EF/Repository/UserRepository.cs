using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FacebookClone.EF.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly IGenericRepository<AppUser> _userRepository;
		private readonly Cloudinary _cloudinary;

		public UserRepository(IGenericRepository<AppUser> userRepository, Cloudinary cloudinary)
		{
			_userRepository = userRepository;
			_cloudinary = cloudinary;
		}


		public async Task SetUserActivationAsync(Guid userId, bool activate)
		{
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				throw new KeyNotFoundException("User not  found");

			if (user.Activated != activate)
			{
				user.Activated = activate;
				await _userRepository.Update(user);
			}

		}

		public async Task AddBio(string bio,string userId)
		{
			if (!Guid.TryParse(userId, out var appUserId))
			{
				throw new ArgumentException("Invalid GUID format");
			}
			var user = (await _userRepository.FindAsync(u => u.Id == appUserId)).FirstOrDefault();

			if (user == null)
				throw new KeyNotFoundException("User not found");

			user.Bio = bio;
			await _userRepository.Update(user);

		}
		public async Task<UserProfileDto> GetUserProfileAsync(string userId)
		{
			if (!Guid.TryParse(userId, out var appUserId))
			{
				throw new ArgumentException("Invalid GUID format");
			}
			var user = (await _userRepository.FindAsync(u => u.Id == appUserId)).FirstOrDefault();

			if (user == null)
				throw new KeyNotFoundException("User not found");

			if (!user.Activated)
				throw new KeyNotFoundException("User not found");

			return new UserProfileDto
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				Bio = user.Bio,
				ProfilePictureUrl = user.ProfilePictureUrl,
				BackgroundPictureUrl = user.BackgroundPictureUrl,
				NumberOfFriends = user.NumberOfFriends
			};
		}
		public async Task UploadProfilePictureAsync(Guid userId, IFormFile file)
		{
			var user = (await _userRepository.FindAsync(u => u.Id == userId)).FirstOrDefault();
			if (user == null) throw new Exception("User not found");

			//hms7 elsora el2adima lw mwgoda
			if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
			{
				var delParams = new DeletionParams(user.ProfilePicturePublicId);
				await _cloudinary.DestroyAsync(delParams);
			}

			var uploadResult = await UploadToCloudinaryAsync(file);
			user.ProfilePictureUrl = uploadResult.Url;
			user.ProfilePicturePublicId = uploadResult.PublicId;

			await _userRepository.Update(user);
			await _userRepository.SaveChangesAsync();
		}

		public async Task UploadBackgroundPictureAsync(Guid userId, IFormFile file)
		{
			var user = (await _userRepository.FindAsync(u => u.Id == userId)).FirstOrDefault();
			if (user == null) throw new Exception("User not found");

			if (!string.IsNullOrEmpty(user.BackgroundPicturePublicId))
			{
				var delParams = new DeletionParams(user.BackgroundPicturePublicId);
				await _cloudinary.DestroyAsync(delParams);
			}

			var uploadResult = await UploadToCloudinaryAsync(file);
			user.BackgroundPictureUrl = uploadResult.Url;
			user.BackgroundPicturePublicId = uploadResult.PublicId;

			await _userRepository.Update(user);
			await _userRepository.SaveChangesAsync();
		}

		public async Task DeleteProfilePictureAsync(Guid userId)
		{
			var user = (await _userRepository.FindAsync(u => u.Id == userId)).FirstOrDefault();
			if (user == null || string.IsNullOrEmpty(user.ProfilePicturePublicId)) return;

			var delParams = new DeletionParams(user.ProfilePicturePublicId);
			await _cloudinary.DestroyAsync(delParams);

			user.ProfilePictureUrl = null;
			user.ProfilePicturePublicId = null;

			await _userRepository.Update(user);
			await _userRepository.SaveChangesAsync();
		}

		public async Task DeleteBackgroundPictureAsync(Guid userId)
		{
			var user = (await _userRepository.FindAsync(u => u.Id == userId)).FirstOrDefault();
			if (user == null || string.IsNullOrEmpty(user.BackgroundPicturePublicId)) return;

			var delParams = new DeletionParams(user.BackgroundPicturePublicId);
			await _cloudinary.DestroyAsync(delParams);

			user.BackgroundPictureUrl = null;
			user.BackgroundPicturePublicId = null;

			await _userRepository.Update(user);
			await _userRepository.SaveChangesAsync();
		}

		private async Task<(string Url, string PublicId)> UploadToCloudinaryAsync(IFormFile file)
		{
			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, file.OpenReadStream()),
				Transformation = new Transformation().Crop("fill").Gravity("auto")
			};

			var result = await _cloudinary.UploadAsync(uploadParams);

			if (result.Error != null)
			{
				throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
			}

			return (result.SecureUrl.ToString(), result.PublicId);
		}
	}
}
