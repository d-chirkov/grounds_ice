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
    using Account_OLD = Model.Entities.Account_OLD;

    [AuthorizeApi]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly IAccountRepository_OLD accountRepository;
        private readonly ILoginValidator_OLD loginValidator;
        private readonly IPasswordValidator_OLD passwordValidator;

        public AccountController(
            IAccountRepository_OLD accountRepository,
            ILoginValidator_OLD loginValidator = null,
            IPasswordValidator_OLD passwordValidator = null)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException("accountRepository");
            this.loginValidator = loginValidator;
            this.passwordValidator = passwordValidator;
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

        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<Value> Register(DTO.LoginAndPassword dto)
        {
            string login = dto?.Login ?? throw new ArgumentNullException("Login");
            string password = dto?.Password ?? throw new ArgumentNullException("Password");
            ValueType result =
                this.loginValidator != null && !await this.loginValidator.ValidateAsync(login) ? ValueType.LoginNotValid :
                this.passwordValidator != null && !await this.passwordValidator.ValidateAsync(password) ? ValueType.PasswordNotValid :
                (await this.accountRepository.CreateAccountAsync(login, password) == null) ? ValueType.LoginAlreadyExists :
                ValueType.Success;
            return new Value((int)result);
        }

        [Route("get_account")]
        [HttpPost]
        public async Task<Value<Account_OLD>> GetAccount()
        {
            long userId = this.GetUserIdFromRequest();
            Account_OLD account = await this.accountRepository.GetAccountAsync(userId);
            return account != null ?
                new Value<Account_OLD>((int)ValueType.Success) { Payload = account } :
                new Value<Account_OLD>((int)ValueType.AccountNotExists);
        }

        [Route("change_login")]
        [HttpPost]
        public async Task<Value> ChangeLogin(DTO.NewLogin dto)
        {
            string newLogin = dto?.Login ?? throw new ArgumentNullException("Login");
            ValueType result;
            long userId = this.GetUserIdFromRequest();
            result =
                this.loginValidator != null && !await this.loginValidator.ValidateAsync(newLogin) ? ValueType.LoginNotValid :
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
                this.passwordValidator != null && !await this.passwordValidator.ValidateAsync(newPassword) ? ValueType.PasswordNotValid :
                !await this.accountRepository.ChangePasswordAsync(userId, oldPassword, newPassword) ? ValueType.OldPasswordNotValid :
                ValueType.Success;
            return new Value((int)result);
        }

        private long GetUserIdFromRequest()
        {
            return (long)(this.Request?.Properties["USER_ID"] ?? throw new ArgumentNullException("USER_ID"));
        }
    }
}
