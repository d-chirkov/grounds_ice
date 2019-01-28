﻿using System.Web.Http;
using Microsoft.Owin;
using Owin;
using Autofac;
using Autofac.Integration.WebApi;
using GroundsIce.Model.Repositories;
using GroundsIce.Model.Abstractions.Repositories;
using System.Net.Http.Formatting;
using GroundsIce.WebApi.Controllers.Account;
using GroundsIce.Model.Validators.Common;
using GroundsIce.Model.Abstractions.Validators;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(GroundsIce.WebApi.Startup))]

namespace GroundsIce.WebApi
{
	public partial class Startup
    {
		public enum CredentialType { Login, Password };

		public void Configuration(IAppBuilder app)
        {
			var builder = new ContainerBuilder();
			builder.RegisterType<InMemoryAccountRepository>().As<IAccountRepository>().InstancePerRequest();

			builder.Register(c => new LengthValidator(5, 20)).Keyed<IStringValidator>(CredentialType.Login).SingleInstance();
			builder.Register(c => new LengthValidator(8, 30)).Keyed<IStringValidator>(CredentialType.Password).SingleInstance();

			builder.RegisterType<AccountController>().OnActivated(c => {
				c.Instance.SetLoginValidators(c.Context.ResolveKeyed<IEnumerable<IStringValidator>>(CredentialType.Login));
				c.Instance.SetPasswordValidators(c.Context.ResolveKeyed<IEnumerable<IStringValidator>>(CredentialType.Password));
			}).InstancePerRequest();

			var config = GlobalConfiguration.Configuration;
			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			config.Formatters.Clear();
			config.Formatters.Add(new JsonMediaTypeFormatter());
			config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());

			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

			ConfigureAuth(app, new InMemoryAccountRepository());
		}
    }
}
