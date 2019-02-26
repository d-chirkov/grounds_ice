namespace GroundsIce.WebApi
{
    using System;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.WebApi.Providers;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.OAuth;
    using Owin;

    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void ConfigureAuth(IAppBuilder app, IAccountRepository_OLD accountRepository)
        {
            string publicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthProvider(publicClientId, accountRepository),
                AuthorizeEndpointPath = new PathString("/api/account/login"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),

                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}