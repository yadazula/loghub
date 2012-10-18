using LogHub.Core.Models;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.ViewModels;
using Raven.Client;

namespace LogHub.Web.Controllers
{
	public class LogDetailController : AbstractApiController
	{
		public LogDetailController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public LogMessageView Get(string id)
		{
			var logMessageView = DocumentSession.Load<LogMessage>(id)
																			    .MapTo<LogMessageView>();

			return logMessageView;
		}
	}
}