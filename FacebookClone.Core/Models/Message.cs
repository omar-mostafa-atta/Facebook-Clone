using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.Models
{
	public class Message:BaseModel
	{
		public int ChatId { get; set; }
		public Chat Chat { get; set; }
		public int SenderId { get; set; }
		public AppUser Sender { get; set; }
		public string Content { get; set; }
		public DateTime SentAt { get; set; } = DateTime.UtcNow;
	}

}
