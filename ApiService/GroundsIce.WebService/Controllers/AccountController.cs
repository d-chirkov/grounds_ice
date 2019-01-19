using System.Threading.Tasks;
using System.Web.Http;
using GroundsIce.WebService.Attributes;
using GroundsIce.Model;
using GroundsIce.Model.Accounting;
using GroundsIce.Model.Repositories;
using System;
using System.Diagnostics;
using System.Net;

namespace GroundsIce.WebService.Controllers
{
    [AuthorizeApi]
    public class AccountController : ApiController
    {
        private IAccountRepository repo_;
        private Registrator userRegistrator_;

        public AccountController(IAccountRepository repo, Registrator userRegistrator)
        {
            repo_ = repo;
            userRegistrator_ = userRegistrator;
        }

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(Credentials credentials)
        {
            Debug.WriteLine($"API:Register | {credentials.Name}, {credentials.Password}");
            try
            {
                Account newAccount = await userRegistrator_.RegisterAsync(credentials);
                return Ok(new { Id = newAccount.User.Id });
            }
            catch (CredentialsValidatorException e)
            {
                return Content(HttpStatusCode.Unauthorized, new { Error = e.Message });
            }
            catch (ArgumentNullException e)
            {
                string missing;
                if (e.Message == "username")
                {
                    missing = "Username";
                }
                else if (e.Message == "password")
                {
                    missing = "Password";
                }
                else
                {
                    return Unauthorized();
                }
                return Content(HttpStatusCode.Unauthorized, new { Error = missing + " is required" });
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetInfo()
        {
            var user = (User)Request.Properties["USER"];
            Debug.WriteLine($"GetInfo: {user.Id}");
            Account foundAccount = await repo_.GetAccountAsync(user);
            Credentials credentials = foundAccount.Credentials;
            Debug.WriteLine($"Found user: {user.Id}, {credentials.Name}, {credentials.Password}");
            return (foundAccount != null) ? Ok(new { Id = user.Id, Name = credentials.Name}) : (IHttpActionResult)Unauthorized();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
