using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace GroundsIce.WebService.Attributes
{
    public class AuthorizeApiAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext))
            {
                return;
            }
            base.OnAuthorization(actionContext);
            string token = string.Empty;
            AuthenticationTicket ticket;

            token = (actionContext.Request.Headers.Any(x => x.Key == "Authorization")) ? actionContext.Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value.SingleOrDefault().Replace("Bearer ", "") : "";

            if (token == string.Empty)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Missing 'Authorization' header. Access denied.");
                return;
            }

            //your OAuth startup class may be called something else...
            ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);

            if (ticket == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token decrypted.");
                return;
            }

            // you could perform some logic on the ticket here...

            // you will be able to retrieve the ticket in all controllers by querying properties and looking for "Ticket"... 
            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("UserId", ticket.Properties.Dictionary["UserId"]));
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}