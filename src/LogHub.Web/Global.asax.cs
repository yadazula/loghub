using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using LogHub.Web.App_Start;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.Models;
using Ninject;
using Raven.Client;
using DependencyResolver = LogHub.Web.Infrastructure.Composition.DependencyResolver;

namespace LogHub.Web
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      AutoMapperConfiguration.Configure();
      //BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}