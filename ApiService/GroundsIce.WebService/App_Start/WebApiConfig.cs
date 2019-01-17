using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

namespace GroundsIce.WebService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //TODO: WTF? [2]
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            // TODO: WTF
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
