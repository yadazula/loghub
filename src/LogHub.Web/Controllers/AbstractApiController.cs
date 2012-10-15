using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using LogHub.Core.Models;
using LogHub.Web.Infrastructure.Common;
using Raven.Client;

namespace LogHub.Web.Controllers
{
	public abstract class AbstractApiController : ApiController
	{
		protected readonly IDocumentSession DocumentSession;

		protected AbstractApiController(IDocumentSession documentSession)
		{
			DocumentSession = documentSession;
		}

		protected User CurrentUser
		{
			get
			{
				var user = DocumentSession.GetUserByUsername(User.Identity.Name);
				return user;
			}
		}

		public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,
		                                                       CancellationToken cancellationToken)
		{
			return base.ExecuteAsync(controllerContext, cancellationToken)
				.ContinueWith(task =>
					{
						using (DocumentSession)
						{
							if (task.Status != TaskStatus.Faulted && DocumentSession != null)
							{
								DocumentSession.SaveChanges();
							}
						}

						return task;
					})
				.Unwrap();
		}
	}
}