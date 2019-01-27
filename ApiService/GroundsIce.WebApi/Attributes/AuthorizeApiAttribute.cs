using System;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;
using GroundsIce.Model.Entities;

namespace GroundsIce.WebApi.Attributes
{
    public class AuthorizeApiAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
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
            long userId = 0;
            if (!long.TryParse(ticket.Properties.Dictionary["USER_ID"], out userId))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token decrypted.");
            }
            actionContext.Request.Properties.Add("USER_ID", userId);
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return 
                actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() || 
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}