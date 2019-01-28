using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroundsIce.WebApi.Controllers.Account.DTO
{
	public class OldAndNewPasswords
	{
		public OldAndNewPasswords(string oldPassword, string newPassword)
		{
			OldPassword = oldPassword;
			NewPassword = newPassword;
		}

		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
}