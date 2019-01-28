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
}