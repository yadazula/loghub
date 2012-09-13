using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using LogHub.Web.Filters;

namespace LogHub.Web
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.Filters.Add( new ApiAuthorizeAttribute());

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
    }
  }
}
