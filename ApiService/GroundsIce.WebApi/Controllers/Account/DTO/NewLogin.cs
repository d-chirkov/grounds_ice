﻿namespace GroundsIce.WebApi.Controllers.Account.DTO
{
	public class NewLogin
	{
		public NewLogin(string login)
		{
			Login = login;
		}

		public string Login { get; set; }
	}
}