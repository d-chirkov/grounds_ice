using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using GroundsIce.Model;
using System;

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
            string token = (actionContext.Request.Headers.Any(x => x.Key == "Authorization")) ? actionContext.Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value.SingleOrDefault().Replace("Bearer ", "") : "";
            if (token == "")
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Missing 'Authorization' header. Access denied.");
                return;
            }
            AuthenticationTicket ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);
            if (ticket == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token decrypted.");
                return;
            }
            var currentUser = new User(UInt64.Parse(ticket.Properties.Dictionary["USER_ID"]));
            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("USER", currentUser));
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}