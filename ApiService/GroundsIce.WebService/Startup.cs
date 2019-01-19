using Autofac;
using Owin;
using Microsoft.Owin;
using GroundsIce.Repositories;
using System.Reflection;
using Autofac.Integration.WebApi;
using System.Web.Http;
using System.Net.Http.Formatting;
using GroundsIce.Model.Accounting;
using GroundsIce.Model.Repositories;
using GroundsIce.Accounting.CredentialsValidators;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(GroundsIce.WebService.Startup))]

namespace GroundsIce.WebService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var repo = new MemoryUsersRepository();
            var builder = new ContainerBuilder();

            builder.Register(c => new MemoryUsersRepository()).As<IAccountRepository>().InstancePerRequest();
            builder.Register(c => new NameLengthValidator(6, 20)).As<ICredentialsValidator>().SingleInstance();
            builder.Register(c => new Registrator(c.Resolve<IAccountRepository>(), c.Resolve<IEnumerable<ICredentialsValidator>>())).InstancePerRequest();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            IContainer container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            GlobalConfiguration.Configuration.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());
            ConfigureAuth(app, repo);
        }
    }
}
