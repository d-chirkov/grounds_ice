using System.Threading.Tasks;
using System.Web.Http;
using GroundsIce.WebService.Attributes;
using GroundsIce.Model;
using GroundsIce.Model.Accounting;
using GroundsIce.Model.Repositories;
using System.Diagnostics;
using System.Net;

namespace GroundsIce.WebService.Controllers
{
    [AuthorizeApi]
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        private AccountService accountService_;

        public AccountController(AccountService accountService) => accountService_ = accountService;

        private enum ErrorCode
        {
            Success = 0,
            Unspecified = 1,
            CredentialsNotValid = 2,
            UserAlreadyExists = 3
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register(string username, string password)
        {
            ErrorCode errorCode = ErrorCode.Unspecified;
            try
            {
                User created = await accountService_.RegisterUserAsync(username, password);
                if (created != null)
                {
                    Ok(new { ErrorCode = ErrorCode.Success, Id = created.Id });
                }
                errorCode = ErrorCode.UserAlreadyExists;
            }
            catch (CredentialsValidationException)
            {
                errorCode = ErrorCode.CredentialsNotValid;
            }
            return Ok(new { ErrorCode = errorCode });
        }

        [HttpPost]
        [Route("get_info")]
        public async Task<IHttpActionResult> GetInfo()
        {
            User user = GetUserFromContext();
            string username = await accountService_.GetUsernameAsync(user);
            return 
                username != null ? Ok(new { ErrorCode = ErrorCode.Success, Id = user.Id, Name = username }) : 
                (IHttpActionResult)Unauthorized();
        }

        private User GetUserFromContext() => (User)Request.Properties["USER"];

        [HttpPost]
        [Route("change_password")]
        public async Task<IHttpActionResult> ChangePassword(string newPassword)
        {
            AccountService.Error error = await accountService_.ChangePasswordAsync(GetUserFromContext(), newPassword);
            return
                error == AccountService.Error.NoError ? Ok(new { ErrorCode = ErrorCode.Success }) :
                error == AccountService.Error.PasswordNotValid ? Ok(new { ErrorCode = ErrorCode.CredentialsNotValid }) :
                (IHttpActionResult)Unauthorized();
        }

        [HttpPost]
        [Route("change_login")]
        public async Task<IHttpActionResult> ChangeLogin(string newLogin)
        {
            AccountService.Error error = await accountService_.ChangeUsernameAsync(GetUserFromContext(), newLogin);
            return
                error == AccountService.Error.NoError ? Ok(new { ErrorCode = ErrorCode.Success }) :
                error == AccountService.Error.RepositoryConflict ? Ok(new { ErrorCode = ErrorCode.UserAlreadyExists }) :
                error == AccountService.Error.UsernameNotValid ? Ok(new { ErrorCode = ErrorCode.CredentialsNotValid }) :
                (IHttpActionResult)Unauthorized();
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
