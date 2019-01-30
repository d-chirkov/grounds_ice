using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.WebApi.Attributes;
using GroundsIce.WebApi.DTO.Common;
using System.Threading.Tasks;

namespace GroundsIce.WebApi.Controllers.Account
{
	using Account = Model.Entities.Account;

    [AuthorizeApi]
	[RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        public enum ValueType
        {
            Success = 1000,
			LoginAlreadyExists = 2000,
			LoginNotValid = 4001,
            PasswordNotValid = 4002,
			AccountNotExists = 5000,
			OldPasswordNotValid = 6000,
        }

        private IEnumerable<IStringValidator> _loginValidators;
        private IEnumerable<IStringValidator> _passwordValidators;
        private IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException("accountRepository");
        }

		public void SetLoginValidators(IEnumerable<IStringValidator> validators)
		{
			CheckValidators(validators);
			_loginValidators = validators;
		}

		public void SetPasswordValidators(IEnumerable<IStringValidator> validators)
		{
			CheckValidators(validators);
			_passwordValidators = validators;
		}

		private void CheckValidators(IEnumerable<IStringValidator> validators)
		{
			if (validators == null || validators.Any(v => v == null)) throw new ArgumentNullException();
		}

		[Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<Value> Register(DTO.LoginAndPassword dto)
        {
			string login = dto?.Login ?? throw new ArgumentNullException("Login");
			string password = dto?.Password ?? throw new ArgumentNullException("Password");
			ValueType result =
				!await IsValueValidated(login, _loginValidators) ? ValueType.LoginNotValid :
				!await IsValueValidated(password, _passwordValidators) ? ValueType.PasswordNotValid :
				(await _accountRepository.CreateAccountAsync(login, password) == null) ? ValueType.LoginAlreadyExists :
				ValueType.Success;
			return new Value((int)result);
        }

        [Route("get_account")]
        [HttpPost]
        public async Task<Value<Account>> GetAccount()
        {
			long userId = GetUserIdFromRequest();
			Account account = await _accountRepository.GetAccountAsync(userId);
			return account != null ? 
				new Value<Account>((int)ValueType.Success) { Payload = account } :
				new Value<Account>((int)ValueType.AccountNotExists);
        }

        [Route("change_login")]
        [HttpPost]
        public async Task<Value> ChangeLogin(DTO.NewLogin dto)
        {
			string newLogin = dto?.Login ?? throw new ArgumentNullException("Login");
			ValueType result;
			long userId = GetUserIdFromRequest();
			result = 
				!await IsValueValidated(newLogin, _loginValidators) ? ValueType.LoginNotValid :
				!await _accountRepository.ChangeLoginAsync(userId, newLogin) ? ValueType.LoginAlreadyExists :
				ValueType.Success;
			return new Value((int)result);
		}

        [Route("change_password")]
        [HttpPost]
        public async Task<Value> ChangePassword(DTO.OldAndNewPasswords dto)
        {
			string oldPassword = dto?.OldPassword ?? throw new ArgumentNullException("OldPassword");
			string newPassword = dto?.NewPassword ?? throw new ArgumentNullException("NewPassword");
			ValueType result;
			long userId = GetUserIdFromRequest();
			result =
				!await IsValueValidated(newPassword, _passwordValidators) ? ValueType.PasswordNotValid :
				!await _accountRepository.ChangePasswordAsync(userId, oldPassword, newPassword) ? ValueType.OldPasswordNotValid :
				ValueType.Success;
			return new Value((int)result);
		}

		private long GetUserIdFromRequest()
		{
			return (long)(Request?.Properties["USER_ID"] ?? throw new ArgumentNullException("USER_ID"));
		}

		private async Task<bool> IsValueValidated(string value, IEnumerable<IStringValidator> validators)
		{
			if (validators != null)
			{
				foreach (IStringValidator validator in validators)
				{
					if (!await validator.ValidateAsync(value))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
