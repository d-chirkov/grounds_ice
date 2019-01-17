using Autofac;
using Owin;
using Microsoft.Owin;
using GroundsIce.Model.Repositories;
using GroundsIce.Repositories;
using System.Reflection;
using Autofac.Integration.WebApi;
using System.Web.Http;

[assembly: OwinStartup(typeof(GroundsIce.WebService.Startup))]

namespace GroundsIce.WebService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var repo = new MemoryUsersRepository();
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<MemoryUsersRepository>().As<IUsersRepository>().InstancePerRequest();

            IContainer container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            ConfigureAuth(app, repo);
        }
    }
}
