using System.Threading.Tasks;
using System.Web.Http;
using GroundsIce.WebService.Attributes;
using GroundsIce.Model;
using GroundsIce.Model.Repositories;
using GroundsIce.Repositories;

namespace GroundsIce.WebService.Controllers
{
    [AuthorizeApi]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private IUsersRepository repo_;

        public AccountController(IUsersRepository repo)
        {
            repo_ = repo;
        }
        
        // POST api/Account/Register
        [HttpGet]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(string userName, string password)
        {
            User newUser = await repo_.AddNewUserAsync(userName, password);
            return (newUser != null) ? Ok() : (IHttpActionResult)Unauthorized();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
