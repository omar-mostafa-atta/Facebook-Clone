using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class ResetPasswordDTO
	{
		public string email { get; set; }
		public string token { get; set; }
		public string newPassword { get; set; }
	}
}
