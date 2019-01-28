using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroundsIce.WebApi.Controllers.Account.DTO
{
	public class LoginAndPassword
	{
		public LoginAndPassword(string login, string password)
		{
			Login = login;
			Password = password;
		}

		public string Login { get; set; }
		public string Password { get; set; }
	}

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

	public class NewLogin
	{
		public NewLogin(string login)
		{
			Login = login;
		}

		public string Login { get; set; }
	}
}