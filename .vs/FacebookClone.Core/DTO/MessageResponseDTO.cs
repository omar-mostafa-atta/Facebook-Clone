using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class MessageResponseDTO
	{
		public string ChatId { get; set; }
		public string SenderId { get; set; }
		public string Content { get; set; }
		public DateTime SentAt { get; set; }
	}
}
