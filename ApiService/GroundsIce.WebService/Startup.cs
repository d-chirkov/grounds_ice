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

[assembly: OwinStartup(typeof(GroundsIce.WebService.Startup))]

namespace GroundsIce.WebService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var repo = new MemoryAccountRepository();
            var builder = new ContainerBuilder();

            builder.Register(c => new MemoryAccountRepository()).As<IAccountRepository>().InstancePerRequest();
            builder.Register(c => {
                var service = new AccountService(c.Resolve<IAccountRepository>());
                service.AddUsernameValidator(new NameLengthValidator(6, 20));
                return service;
                }).InstancePerRequest();

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
