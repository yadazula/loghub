using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LogHub.Web.Infrastructure;
using LogHub.Web.Models;
using LogHub.Web.ViewModels;

namespace LogHub.Web.Controllers
{
  [AllowAnonymous]
  public class LoginController : Controller
  {
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

      var user = FakeUserService.Get(loginInput.Username, loginInput.Password);
      if (user == null)
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