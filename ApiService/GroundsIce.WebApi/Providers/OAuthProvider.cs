namespace GroundsIce.WebApi.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Entities;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;

    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string publicClientId;
        private readonly IAccountRepository accountRepository;

        public OAuthProvider(string publicClientId, IAccountRepository accountRepository)
        {
            this.publicClientId = publicClientId ?? throw new ArgumentNullException("publicClientId");
            this.accountRepository = accountRepository ?? throw new ArgumentNullException("accountRepository");
        }

        public static AuthenticationProperties CreateProperties(long userId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "USER_ID", userId.ToString() }
            };
            return new AuthenticationProperties(data);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            Account account = await this.accountRepository.GetAccountAsync(context.UserName, context.Password);
            if (account == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var userId = account.UserId;
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, context.UserName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var oAuthIdentity = new ClaimsIdentity(claims, "Bearer");
            AuthenticationProperties properties = CreateProperties(userId);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == this.publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }
    }
}