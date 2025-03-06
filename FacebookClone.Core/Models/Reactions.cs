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
		
		public int AppUserId { get; set; }
		public AppUser AppUser { get; set; }

		public int PostId { get; set; }
		public Post Post { get; set; }

	}
}
