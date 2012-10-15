using System.Web.Http;
using LogHub.Web.Filters;
using LogHub.Web.Infrastructure.Composition;
using Newtonsoft.Json.Serialization;

namespace LogHub.Web.App_Start
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.DependencyResolver = new DependencyResolver(NinjectWebCommon.Kernel);

			config.Filters.Add(new ApiAuthorizeAttribute());
			config.Filters.Add(new ValidationFilterAttribute());

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new {id = RouteParameter.Optional}
				);
		}
	}
}