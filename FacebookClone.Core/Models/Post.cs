using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.Models
{
	public class Post : BaseModel
	{
	 
		public string Text { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; }

		public int TotalReactions { get; set; }

		public Guid AppUserId { get; set; }
		public AppUser AppUser { get; set; }

		public List<Comment> Comments { get; set; }=new List<Comment>();
		public List<Reactions> Reactions { get; set; }= new List<Reactions>();
		public List<Media> Media { get; set; } = new List<Media>();

		public Guid? SharedPostId { get; set; }
		public Post? SharedPost { get; set; }
	}	
}
