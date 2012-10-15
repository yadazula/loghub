using LogHub.Core.Extensions;
using LogHub.Core.Models;
using LogHub.Web.Infrastructure.Common;
using Raven.Client;

namespace LogHub.Web.Infrastructure.Composition.Tasks
{
	public class CheckIfAdminUserExist : IStartupTask
	{
		private readonly IDocumentStore documentStore;

		public CheckIfAdminUserExist(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
		}

		public void Execute()
		{
			using (var documentSession = documentStore.OpenSession())
			{
				var admin = documentSession.GetUserByUsername(User.Admin);

				if (admin.IsNull())
				{
					var user = new User
						{
							Username = User.Admin,
							Name = "Administrator",
							Role = UserRole.Administrator,
							Email = "admin@loghub.com"
						};

					user.SetPassword("loghub");

					documentSession.Store(user);
					documentSession.SaveChanges();
				}
			}
		}
	}
}