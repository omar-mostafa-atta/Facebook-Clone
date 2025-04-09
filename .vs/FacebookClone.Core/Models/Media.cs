using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.Models
{
	public class Media : BaseModel
	{
		 
		public string Type { get; set; }
		public string Url { get; set; } 
		public string PublicId { get; set; }

		public Guid PostId { get; set; }
		public Post Post { get; set; }
	}
}
