using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class MediaDto
	{
		public Guid Id { get; set; }
		public string Type { get; set; }
		public string Url { get; set; }
		public string PublicId { get; set; }
		public Guid PostId { get; set; }
		 
	}
}
