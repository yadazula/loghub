using System.Collections.Generic;
using System.Linq;
using LogHub.Core.Models;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.ViewModels;
using Raven.Client;

namespace LogHub.Web.Controllers
{
	public class AlertController : AbstractApiController
	{
		public AlertController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public IEnumerable<LogAlertView> Get()
		{
			var items = DocumentSession.Query<LogAlert>()
				.Where(x => x.User == CurrentUser.Id)
				.ToList();

			var result = items.MapTo<LogAlertView>();
			return result;
		}

		public void Post(LogAlertView logAlertView)
		{
			Store(logAlertView);
		}

		public void Put(LogAlertView logAlertView)
		{
			Store(logAlertView);
		}

		public void Delete(string id)
		{
			var item = DocumentSession.Load<LogAlert>(id);
			DocumentSession.Delete(item);
		}

		private void Store(LogAlertView logAlertView)
		{
			var logAlert = logAlertView.MapTo<LogAlert>();
			logAlert.User = CurrentUser.Id;
			DocumentSession.Store(logAlert);
		}
	}
}