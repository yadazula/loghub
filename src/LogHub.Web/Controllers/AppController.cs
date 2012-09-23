using System.Linq;
using System.Web.Mvc;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.Infrastructure.Common;
using Raven.Client;

namespace LogHub.Web.Controllers
{
  public class AppController : AbstractController
  {
    public AppController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public ActionResult Index()
    {
      var user = DocumentSession.Query<User>().FirstOrDefault(x => x.Username == User.Identity.Name);
      if (user.IsNull())
      {
        return View("Error");
      }

      ViewBag.User = user.Name;
      return View();
    }
  }
}