using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LogHub.Web.Models;

namespace LogHub.Web.Filters
{
  public class CustomAuthorizeAttribute : AuthorizeAttribute
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
        var userPrincipal = new CustomPrincipal(formsIdentity, authTicket.UserData);
        httpContext.User = userPrincipal;
      }
      return isAuthenticated;
    }
  }

  public class CustomPrincipal : IPrincipal
  {
    private readonly UserRole currentRole;
    public IIdentity Identity { get; private set; }

    public CustomPrincipal(IIdentity identity, string role)
    {
      currentRole = (UserRole)Enum.Parse(typeof(UserRole), role);
      Identity = identity;
    }

    public bool IsInRole(string role)
    {
      var targetRole = (UserRole)Enum.Parse(typeof(UserRole), role);

      if (targetRole == currentRole)
      {
        return true;
      }

      if (targetRole == UserRole.Reader && currentRole == UserRole.Administrator)
      {
        return true;
      }

      return false;
    }
  }
}