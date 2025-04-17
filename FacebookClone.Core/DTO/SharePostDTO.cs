using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class SharePostDTO
	{
		public string OriginalPostId { get; set; }
		public string? Text { get; set; }
		public List<IFormFile>? MediaFiles { get; set; } 
	}
}
