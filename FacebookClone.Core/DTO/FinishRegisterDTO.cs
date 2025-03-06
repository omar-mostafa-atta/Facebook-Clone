using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class FinishRegisterDTO
	{
		public string EmailToken { get; set; }
		public string otp { get; set; }
	}
}
