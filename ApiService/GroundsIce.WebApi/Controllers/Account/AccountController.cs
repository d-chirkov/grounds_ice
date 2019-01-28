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
        public enum ValueType : int
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
            _loginValidators = new List<IStringValidator>();
            _passwordValidators = new List<IStringValidator>();
        }

        public void SetLoginValidators(IEnumerable<IStringValidator> loginValidators)
		{
			if (loginValidators == null || loginValidators.Any(v => v == null)) throw new ArgumentNullException();
			_loginValidators = loginValidators;
        }

		public void SetPasswordValidators(IEnumerable<IStringValidator> passwordValidators)
		{
			if (passwordValidators == null || passwordValidators.Any(v => v == null)) throw new ArgumentNullException();
			_passwordValidators = passwordValidators;
		}

		[Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<Value> Register(DTO.LoginAndPassword loginAndPasswordDto)
        {
			string login = loginAndPasswordDto?.Login ?? throw new ArgumentNullException("Login");
			string password = loginAndPasswordDto?.Password ?? throw new ArgumentNullException("Password");
			ValueType result =
				!await IsValueValidated(login, _loginValidators) ? ValueType.LoginNotValid :
				!await IsValueValidated(password, _passwordValidators) ? ValueType.PasswordNotValid :
				(await _accountRepository.CreateAccountAsync(login, password) == null) ? ValueType.LoginAlreadyExists :
				ValueType.Success;
			return new Value((int)result);
        }

		private async Task<bool> IsValueValidated(string value, IEnumerable<IStringValidator> validators)
		{
			foreach (IStringValidator validator in validators)
			{
				if (!await validator.ValidateAsync(value))
				{
					return false;
				}
			}
			return true;
		}

		private long GetUserIdFromRequest()
		{
			return (long)(Request?.Properties["USER_ID"] ?? throw new ArgumentNullException("USER_ID"));
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
        public async Task<Value> ChangeLogin(DTO.NewLogin newLoginDto)
        {
			string newLogin = newLoginDto?.Login ?? throw new ArgumentNullException("Login");
			return await ChangeCredentials(async (long userId) =>
			{
				 return
					 !await IsValueValidated(newLogin, _loginValidators) ? ValueType.LoginNotValid :
					 !await _accountRepository.ChangeLoginAsync(userId, newLogin) ? ValueType.LoginAlreadyExists :
					 ValueType.Success;
			});
        }

        [Route("change_password")]
        [HttpPost]
        public async Task<Value> ChangePassword(DTO.OldAndNewPasswords oldAndNewPasswordsDto)
        {
			string oldPassword = oldAndNewPasswordsDto?.OldPassword ?? throw new ArgumentNullException("OldPassword");
			string newPassword = oldAndNewPasswordsDto?.NewPassword ?? throw new ArgumentNullException("NewPassword");
			return await ChangeCredentials(async (long userId) =>
			{
				if (!await IsValueValidated(newPassword, _passwordValidators))
				{
					return ValueType.PasswordNotValid;
				}
				bool changed = await _accountRepository.ChangePasswordAsync(userId, oldPassword, newPassword);
				return changed ? ValueType.Success : ValueType.OldPasswordNotValid;
			});
		}

		private async Task<Value> ChangeCredentials(Func<long, Task<ValueType>> change)
		{
			long userId = GetUserIdFromRequest();
			ValueType result;
			try
			{
				result = await change(userId);
			}
			catch (UserIdNotFoundException)
			{
				result = ValueType.AccountNotExists;
			}
			return new Value((int)result);
		}
	}
}
