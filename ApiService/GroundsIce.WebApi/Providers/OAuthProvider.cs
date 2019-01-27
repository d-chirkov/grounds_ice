using System;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using GroundsIce.Model.Abstractions.Repositories;
using System.Threading.Tasks;
using System.Security.Claims;
using GroundsIce.Model.Entities;

namespace GroundsIce.WebApi.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private IAccountRepository _accountRepository;

        public OAuthProvider(string publicClientId, IAccountRepository accountRepository)
        {
            _publicClientId = publicClientId ?? throw new ArgumentNullException("publicClientId");
            _accountRepository = accountRepository ?? throw new ArgumentNullException("publicClientId");
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            Account account = await _accountRepository.GetAccountAsync(context.UserName, context.Password);
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