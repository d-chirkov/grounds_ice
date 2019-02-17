namespace GroundsIce.WebApi.Attributes
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using Microsoft.Owin.Security;

    public class AuthorizeApiAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            base.OnAuthorization(actionContext);
            string token = actionContext.Request.Headers.Any(x => x.Key == "Authorization") ? actionContext.Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value.SingleOrDefault().Replace("Bearer ", string.Empty) : string.Empty;
            if (token == string.Empty)
            {
                if (!IsAnonymous(actionContext))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Missing 'Authorization' header. Access denied.");
                }

                return;
            }

            AuthenticationTicket ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);
            if (ticket == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token decrypted.");
                return;
            }

            if (!long.TryParse(ticket.Properties.Dictionary["USER_ID"], out long userId))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid token decrypted.");
            }

            actionContext.Request.Properties.Add("USER_ID", (long)0);
        }

        private static bool IsAnonymous(HttpActionContext actionContext)
        {
            return
                actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}