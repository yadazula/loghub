using System.Web.Mvc;
using Raven.Client;

namespace LogHub.Web.Controllers
{
  public abstract class AbstractController  : Controller
  {
    protected readonly IDocumentSession DocumentSession;

    protected AbstractController(IDocumentSession documentSession)
    {
      DocumentSession = documentSession;
    }
  }
}