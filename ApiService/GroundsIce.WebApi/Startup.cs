using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using GroundsIce.Model.Abstractions;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.ConnectionFactories;
using GroundsIce.Model.Entities;
using GroundsIce.Model.Repositories;
using GroundsIce.Model.Validators;
using GroundsIce.WebApi.Controllers.Account;
using GroundsIce.WebApi.Controllers.Profile;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GroundsIce.WebApi.Startup))]

namespace GroundsIce.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB;Integrated Security=True;Pooling=False";
            builder.Register(c => new SqlConnectionFactory(sqlConnectionString)).As<IConnectionFactory>().SingleInstance();

            builder.RegisterType<DbAccountRepository_OLD>().As<IAccountRepository_OLD>().InstancePerRequest();
            builder.RegisterType<DbProfileRepository>().As<IProfileRepository>().InstancePerRequest();

            builder.Register(c => new LoginValidator(5, 20)).As<ILoginValidator>();
            builder.Register(c => new PasswordValidator(8, 30)).As<IPasswordValidator>();

            builder.Register(c =>
            {
                return new ProfileInfoValidator()
                {
                    TypesMaxLengths = new Dictionary<ProfileInfoType, int>()
                    {
                        { ProfileInfoType.FirstName, 30 },
                        { ProfileInfoType.LastName, 30 },
                        { ProfileInfoType.MiddleName, 30 },
                        { ProfileInfoType.City, 35 },
                        { ProfileInfoType.Region, 35 },
                        { ProfileInfoType.Description, 300 },
                    }
                };
            }).As<IProfileInfoValidator>().SingleInstance();

            // TODO: add whitespace checks for login and profile info
            builder.RegisterType<AccountController>().InstancePerRequest();
            builder.RegisterType<ProfileController>().InstancePerRequest();

            var config = GlobalConfiguration.Configuration;
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            this.ConfigureAuth(app, new DbAccountRepository_OLD(container.Resolve<IConnectionFactory>()));
        }
    }
}
