using System.Web.Mvc;
using Raven.Client;

namespace LogHub.Web.Controllers
{
	public abstract class AbstractMvcController : Controller
	{
		protected readonly IDocumentSession DocumentSession;

		protected AbstractMvcController(IDocumentSession documentSession)
		{
			DocumentSession = documentSession;
		}
	}
}