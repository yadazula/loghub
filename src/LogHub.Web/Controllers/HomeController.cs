using System.Linq;
using System.Web.Mvc;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.Infrastructure.Common;
using Raven.Client;

namespace LogHub.Web.Controllers
{
  public class HomeController : AbstractMvcController
  {
    public HomeController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public ActionResult Index()
    {
      var user = DocumentSession.GetUserByUsername(User.Identity.Name);
      if (user.IsNull())
      {
        return View("Error");
      }

      ViewBag.User = user.Name;
      return View();
    }
  }
}