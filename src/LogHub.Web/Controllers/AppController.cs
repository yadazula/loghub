using System.Web.Mvc;

namespace LogHub.Web.Controllers
{
  public class AppController : Controller
  {
     public ActionResult Index()
     {
       return View();
     }
  }
}