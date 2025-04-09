using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Http;

namespace FacebookClone.EF.Repository
{
	public class MediaRepository:IMediaRepository
	{
		private readonly IGenericRepository<Media> _mediaRepository;
		private readonly Cloudinary _cloudinary;

		public MediaRepository(IGenericRepository<Media> mediaRepository, Cloudinary cloudinary)
		{
			_mediaRepository = mediaRepository;
			_cloudinary = cloudinary;
		}

		public async Task<Media> UploadMediaAsync(IFormFile file, Guid postId)
		{
			if (file == null || file.Length == 0)
			{
				throw new ArgumentException("No file provided.");
			}

			
			string type = file.ContentType.StartsWith("image") ? "image" : "video";

			
			var uploadParams = new RawUploadParams
			{
				File = new FileDescription(file.FileName, file.OpenReadStream())
				
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);

			if (uploadResult.Error != null)
			{
				throw new Exception($"Upload failed: {uploadResult.Error.Message}");
			}

			
			var media = new Media
			{
				Type = type,
				Url = uploadResult.SecureUrl.ToString(),
				PublicId = uploadResult.PublicId,
				PostId = postId
			};

			
			await _mediaRepository.AddAsync(media);
			await _mediaRepository.SaveChangesAsync();

			return media;
		}

		public async Task<string> GetMediaUrlAsync(Guid mediaId)
		{
			var media = (await _mediaRepository.FindAsync(m => m.Id == mediaId)).FirstOrDefault();
			if (media == null)
			{
				throw new Exception("Media not found.");
			}
			return media.Url;
		}

		public async Task DeleteMediaAsync(Guid mediaId)
		{
			var media = (await _mediaRepository.FindAsync(m => m.Id == mediaId)).FirstOrDefault();
			if (media == null)
			{
				return;
			}

			
			var deletionParams = new DeletionParams(media.PublicId)
			{
				ResourceType = media.Type.ToLower() == "image" ? ResourceType.Image : ResourceType.Video
			};
			await _cloudinary.DestroyAsync(deletionParams);

			
			_mediaRepository.Delete(media);
			await _mediaRepository.SaveChangesAsync();
		}

	}
}
