using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using GroundsIce.Model;
using GroundsIce.Model.Repositories;

namespace GroundsIce.WebService.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private IAccountRepository repo_;

        public ApplicationOAuthProvider(string publicClientId, IAccountRepository repo)
        {
            _publicClientId = publicClientId ?? throw new ArgumentNullException("publicClientId");
            repo_ = repo;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            User user = await repo_.GetUserAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, context.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var oAuthIdentity = new ClaimsIdentity(claims, "Bearer");
            AuthenticationProperties properties = CreateProperties(user.Id);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            // TODO: WTF?
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
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(long userId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "USER_ID", userId.ToString() }
            };
            return new AuthenticationProperties(data);
        }
    }
}