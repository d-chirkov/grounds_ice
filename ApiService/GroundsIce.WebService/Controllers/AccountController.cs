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
    //[RoutePrefix("api/account")]
    [AuthorizeApi]
    public class AccountController : ApiController
    {
        private AccountService accountService_;

        public AccountController(AccountService accountService) => accountService_ = accountService;

        private enum ErrorCode
        {
            Success = 1000,
            Unspecified = 2000,
            CredentialsNotValid = 3000,
            UserAlreadyExists = 4000,
        }

        public struct UserData
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        //[Route("~api/account/register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> Register(UserData userData)
        {
            string username = userData.username;
            string password = userData.password;
            Debug.WriteLine("DEBUG_OUTPUT: Register");
            ErrorCode errorCode = ErrorCode.Unspecified;
            try
            {
                User created = await accountService_.RegisterUserAsync(username, password);
                if (created != null)
                {
                    return Ok(new { ErrorCode = ErrorCode.Success, Id = created.Id });
                }
                errorCode = ErrorCode.UserAlreadyExists;
            }
            catch (CredentialsValidationException)
            {
                errorCode = ErrorCode.CredentialsNotValid;
            }
            return Ok<object>(new { ErrorCode = errorCode });
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetInfo()
        {
            Debug.WriteLine("DEBUG_OUTPUT: GetInfo");
            User user = GetUserFromContext();
            string username = await accountService_.GetUsernameAsync(user);
            return 
                username != null ?
                Ok<object>(new { ErrorCode = ErrorCode.Success, Id = user.Id, Name = username }) :
                Ok<object>(new { ErrorCode = ErrorCode.Unspecified });
        }

        private User GetUserFromContext() => (User)Request.Properties["USER"];

        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(string newPassword)
        {
            AccountService.Error error = await accountService_.ChangePasswordAsync(GetUserFromContext(), newPassword);
            return
                error == AccountService.Error.NoError ? Ok(new { ErrorCode = ErrorCode.Success }) :
                error == AccountService.Error.PasswordNotValid ? Ok(new { ErrorCode = ErrorCode.CredentialsNotValid }) :
                Ok(new { ErrorCode = ErrorCode.Unspecified });
        }

        [HttpPost]
        public async Task<IHttpActionResult> ChangeLogin(string newLogin)
        {
            AccountService.Error error = await accountService_.ChangeUsernameAsync(GetUserFromContext(), newLogin);
            return
                error == AccountService.Error.NoError ? Ok<object>(new { ErrorCode = ErrorCode.Success }) :
                error == AccountService.Error.RepositoryConflict ? Ok<object>(new { ErrorCode = ErrorCode.UserAlreadyExists }) :
                error == AccountService.Error.UsernameNotValid ? Ok<object>(new { ErrorCode = ErrorCode.CredentialsNotValid }) :
                Ok<object>(new { ErrorCode = ErrorCode.Unspecified });
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
