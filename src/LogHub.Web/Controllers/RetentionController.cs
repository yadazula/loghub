using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using LogHub.Core.Models;
using LogHub.Web.Filters;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Web.Controllers
{
	[ApiAuthorize(Roles = "Administrator")]
	public class RetentionController : AbstractApiController
	{
		public RetentionController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public IEnumerable<Retention> Get()
		{
			var items = DocumentSession.Query<Retention>()
				.Customize(x => x.WaitForNonStaleResultsAsOfNow())
				.ToList();

			return items;
		}

		public void Post(Retention retention)
		{
			Store(retention);
		}

		public void Put(Retention retention)
		{
			Store(retention);
		}

		public void Delete(string id)
		{
			var item = DocumentSession.Load<Retention>(id);
			DocumentSession.Delete(item);
		}

		private void Store(Retention retention)
		{
			retention.CreatedBy = User.Identity.Name;
			retention.CreatedAt = DateTimeOffset.Now;
			DocumentSession.Store(retention);
		}
	}
}