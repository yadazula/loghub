using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LogHub.Web.Infrastructure;
using LogHub.Web.Models;
using LogHub.Web.ViewModels;
using Raven.Client;
using Raven.Client.Document;

namespace LogHub.Web.Controllers
{
  [AllowAnonymous]
  public class LoginController : AbstractController
  {
    public LoginController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    [HttpGet]
    public ActionResult Index()
    {
      if (Request.IsAuthenticated)
      {
        return RedirectToAction("Index", "App");
      }

      return View();
    }

    [HttpPost]
    public ActionResult SignIn(LoginInput loginInput)
    {
      if (!ModelState.IsValid)
      {
        TempData["HasErrors"] = true;
        return RedirectToAction("Index");
      }

      var user = DocumentSession.Query<User>()
                                .FirstOrDefault(x => x.Username == loginInput.Username);

      if (user == null || !user.ValidatePassword(loginInput.Password))
      {
        TempData["HasErrors"] = true;
        return RedirectToAction("Index");
      }

      var authTicket = new FormsAuthenticationTicket(1, loginInput.Username, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), true, user.Role.ToString());
      var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
      Response.AppendCookie(authCookie);

      return RedirectToAction("Index", "App");
    }

    [HttpGet]
    public ActionResult SignOut()
    {
      FormsAuthentication.SignOut();
      return RedirectToAction("Index");
    }
  }
}