using System.Web.Http;
using Microsoft.Owin;
using Owin;
using Autofac;
using Autofac.Integration.WebApi;
using GroundsIce.Model.Repositories;
using GroundsIce.Model.Abstractions.Repositories;
using System.Net.Http.Formatting;
using GroundsIce.Model.Validators.Common;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.ConnectionFactories;
using GroundsIce.Model.Abstractions;
using System.Collections.Generic;
using GroundsIce.Model.Validators;
using GroundsIce.WebApi.Controllers.Account;
using GroundsIce.WebApi.Controllers.Profile;
using GroundsIce.Model.Validators.ProfileInfo;

[assembly: OwinStartup(typeof(GroundsIce.WebApi.Startup))]

namespace GroundsIce.WebApi
{
	public partial class Startup
    {
		public enum CredentialType { Login, Password };

		public void Configuration(IAppBuilder app)
        {
			var builder = new ContainerBuilder();
			string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB;Integrated Security=True;Pooling=False";
			builder.Register(c => new SqlConnectionFactory(sqlConnectionString)).As<IConnectionFactory>().SingleInstance();

			builder.RegisterType<DbAccountRepository>().As<IAccountRepository>().InstancePerRequest();
			builder.RegisterType<DbProfileRepository>().As<IProfileRepository>().InstancePerRequest();

			builder.Register(c => new LengthValidator(5, 20)).Keyed<IStringValidator>(CredentialType.Login).SingleInstance();
			builder.Register(c => new LengthValidator(8, 30)).Keyed<IStringValidator>(CredentialType.Password).SingleInstance();

			builder.RegisterType<UniqueTypesValidator>().As<IProfileInfoValidator>();
			builder.RegisterType<NoEmptyFieldsValidator>().As<IProfileInfoValidator>();
			//TODO: add checking for field length in IProfileInfoValidator

			builder.RegisterType<ProfileController>().InstancePerRequest();

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

			ConfigureAuth(app, new DbAccountRepository(container.Resolve<IConnectionFactory>()));
		}
    }
}
