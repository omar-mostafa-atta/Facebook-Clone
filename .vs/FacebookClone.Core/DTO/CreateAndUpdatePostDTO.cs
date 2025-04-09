using Microsoft.AspNetCore.Http;

namespace FacebookClone.Core.DTO
{
	public class CreateAndUpdatePostDTO
	{
		public string Text { get; set; }
		public List<IFormFile>? MediaFiles { get; set; }
	}
}
