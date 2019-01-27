using Autofac;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web;
using System.Web.Http;
using GroundsIce.Model.Repositories;
using GroundsIce.Model.Abstractions.Repositories;
using System.Net.Http.Formatting;
using GroundsIce.WebApi.Controllers.Account;
using GroundsIce.Model.Validators.Common;
using GroundsIce.Model.Abstractions.Validators;
using System.Collections.Generic;

namespace GroundsIce.WebApi
{
    public class WebApiApplication : HttpApplication
    {
		public enum CredentialType { Login, Password };

        protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			var builder = new ContainerBuilder();
			builder.RegisterType<InMemoryAccountRepository>().As<IAccountRepository>();


			builder.Register(c => new LengthValidator(5, 20)).Keyed<IStringValidator>(CredentialType.Login);
			builder.Register(c => new LengthValidator(8, 30)).Keyed<IStringValidator>(CredentialType.Password);

			builder.RegisterType<AccountController>().InstancePerRequest().OnActivated(c => {
				c.Instance.SetLoginValidators(c.Context.ResolveKeyed<IEnumerable<IStringValidator>>(CredentialType.Login));
				c.Instance.SetPasswordValidators(c.Context.ResolveKeyed<IEnumerable<IStringValidator>>(CredentialType.Password));
			});

			var config = GlobalConfiguration.Configuration;
			//builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			GlobalConfiguration.Configuration.Formatters.Clear();
			GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
			GlobalConfiguration.Configuration.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());

        }
    }
}
