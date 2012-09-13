using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LogHub.Web.Filters
{
  public class MVCAuthorizeAttribute : AuthorizeAttribute
  {
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      var isAuthenticated = base.AuthorizeCore(httpContext);
      if (isAuthenticated)
      {
        var cookieName = FormsAuthentication.FormsCookieName;
        if (!httpContext.User.Identity.IsAuthenticated ||
            httpContext.Request.Cookies == null ||
            httpContext.Request.Cookies[cookieName] == null)
        {
          return false;
        }

        var authCookie = httpContext.Request.Cookies[cookieName];
        var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        var formsIdentity = new FormsIdentity(authTicket);
        var userPrincipal = new LogHubPrincipal(formsIdentity, authTicket.UserData);
        httpContext.User = userPrincipal;
      }
      return isAuthenticated;
    }
  }
}