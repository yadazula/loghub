using System.Web.Mvc;
using LogHub.Web.Filters;

namespace LogHub.Web.App_Start
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
      filters.Add(new MVCAuthorizeAttribute());
    }
  }
}