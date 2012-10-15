using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
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
			if (!CheckRoles(userPrincipal) || !CheckUsers(userPrincipal))
			{
				HandleUnauthorizedRequest(actionContext);
			}
		}

		protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			actionContext.Response = new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.Unauthorized,
					RequestMessage = actionContext.ControllerContext.Request
				};
		}

		private bool CheckRoles(IPrincipal principal)
		{
			var roles = SplitByComma(Roles);
			if (roles.Length == 0) return true;
			return roles.Any(principal.IsInRole);
		}

		private bool CheckUsers(IPrincipal principal)
		{
			var users = SplitByComma(Users);
			if (users.Length == 0) return true;
			return users.Any(user => principal.Identity.Name == user);
		}

		protected static string[] SplitByComma(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return new string[0];

			var roles = input.Split(',')
				.Where(role => !String.IsNullOrWhiteSpace(role.Trim()));

			return roles.Select(role => role.Trim()).ToArray();
		}
	}
}