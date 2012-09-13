using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace LogHub.Web.Filters
{
  public class ApiAuthorizeAttribute : AuthorizeAttribute
  {
    public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
    {
      var cookieName = FormsAuthentication.FormsCookieName;
      
      if (!HttpContext.Current.User.Identity.IsAuthenticated ||
          HttpContext.Current.Request.Cookies[cookieName] == null)
      {
        HandleUnauthorizedRequest(actionContext);
        return;
      }

      var authCookie = HttpContext.Current.Request.Cookies[cookieName];
      var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
      var formsIdentity = new FormsIdentity(authTicket);
      var userPrincipal = new LogHubPrincipal(formsIdentity, authTicket.UserData);
      HttpContext.Current.User = userPrincipal;  
    }

    protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
    {
      actionContext.Response = new HttpResponseMessage
      {
        StatusCode = HttpStatusCode.Unauthorized,
        RequestMessage = actionContext.ControllerContext.Request
      };
    }
  }
}