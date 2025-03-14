using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.IRepository
{
	public interface IMediaRepository
	{
		Task<Media> UploadMediaAsync(IFormFile file, Guid postId);
		Task<string> GetMediaUrlAsync(Guid mediaId);
		Task DeleteMediaAsync(Guid mediaId);
	}
}
