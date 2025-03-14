using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class SignUpResponseDto
	{
		public string OTP { get; set; }
		public string EmailOnlyToken { get; set; }
	}
}
