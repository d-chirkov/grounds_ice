namespace GroundsIce.WebApi.Controllers.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.WebApi.Attributes;
    using GroundsIce.WebApi.DTO.Common;
    using Account = Model.Entities.Account;

    [AuthorizeApi]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly IAccountRepository accountRepository;
        private IEnumerable<IStringValidator> loginValidators;
        private IEnumerable<IStringValidator> passwordValidators;

        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException("accountRepository");
        }

        public enum ValueType
        {
            Success = 1000,
            LoginAlreadyExists = 2000,
            LoginNotValid = 4001,
            PasswordNotValid = 4002,
            AccountNotExists = 5000,
            OldPasswordNotValid = 6000,
        }

        public void SetLoginValidators(IEnumerable<IStringValidator> validators)
        {
            this.CheckValidators(validators);
            this.loginValidators = validators;
        }

        public void SetPasswordValidators(IEnumerable<IStringValidator> validators)
        {
            this.CheckValidators(validators);
            this.passwordValidators = validators;
        }

        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<Value> Register(DTO.LoginAndPassword dto)
        {
            string login = dto?.Login ?? throw new ArgumentNullException("Login");
            string password = dto?.Password ?? throw new ArgumentNullException("Password");
            ValueType result =
                !await this.IsValueValidated(login, this.loginValidators) ? ValueType.LoginNotValid :
                !await this.IsValueValidated(password, this.passwordValidators) ? ValueType.PasswordNotValid :
                (await this.accountRepository.CreateAccountAsync(login, password) == null) ? ValueType.LoginAlreadyExists :
                ValueType.Success;
            return new Value((int)result);
        }

        [Route("get_account")]
        [HttpPost]
        public async Task<Value<Account>> GetAccount()
        {
            long userId = this.GetUserIdFromRequest();
            Account account = await this.accountRepository.GetAccountAsync(userId);
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
            long userId = this.GetUserIdFromRequest();
            result =
                !await this.IsValueValidated(newLogin, this.loginValidators) ? ValueType.LoginNotValid :
                !await this.accountRepository.ChangeLoginAsync(userId, newLogin) ? ValueType.LoginAlreadyExists :
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
            long userId = this.GetUserIdFromRequest();
            result =
                !await this.IsValueValidated(newPassword, this.passwordValidators) ? ValueType.PasswordNotValid :
                !await this.accountRepository.ChangePasswordAsync(userId, oldPassword, newPassword) ? ValueType.OldPasswordNotValid :
                ValueType.Success;
            return new Value((int)result);
        }

        private void CheckValidators(IEnumerable<IStringValidator> validators)
        {
            if (validators == null || validators.Any(v => v == null))
            {
                throw new ArgumentNullException("validators");
            }
        }

        private long GetUserIdFromRequest()
        {
            return (long)(this.Request?.Properties["USER_ID"] ?? throw new ArgumentNullException("USER_ID"));
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
