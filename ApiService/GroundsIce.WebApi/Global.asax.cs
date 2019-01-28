using Autofac;
using Autofac.Integration.WebApi;
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
		

        protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			//TODO: uncomment in production
			//GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;
		}
    }
}
