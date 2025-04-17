using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class PostDTO
	{
		public Guid Id { get; set; }
		public string Text { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public int TotalReactions { get; set; }
		public int TotalComments { get; set; }
		public int TotalShares { get; set; }
		public Guid? SharedPostId { get; set; }
		public Guid AppUserId { get; set; }
		public PostDTO? SharedPost { get; set; }
		public List<MediaDto> Media { get; set; } = new List<MediaDto>();
	}
}
