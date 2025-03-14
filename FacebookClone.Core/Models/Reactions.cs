using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.Models
{
	public class Reactions : BaseModel
	{
		 
		public string Type { get; set; }
		
		public Guid AppUserId { get; set; }
		public AppUser AppUser { get; set; }

		public Guid PostId { get; set; }
		public Post Post { get; set; }

	}
}
