using System;
using System.Linq;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Tasks.Startup
{
	public class CheckInitialData : IStartupTask
	{
		private readonly IDocumentStore documentStore;

		public CheckInitialData(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
		}

		public void Execute()
		{
			using (var documentSession = documentStore.OpenSession())
			{
				CheckDefaultAdminUser(documentSession);
				CheckDefaultRetentionRule(documentSession);
				CheckThroughputInfo(documentSession);
				
				documentSession.SaveChanges();
			}
		}

		private static void CheckDefaultAdminUser(IDocumentSession documentSession)
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
			}
		}

		private void CheckDefaultRetentionRule(IDocumentSession documentSession)
		{
			var retention = documentSession.Query<Retention>().FirstOrDefault();

			if (retention.IsNull())
			{
				retention = new Retention
				{
					Days = 60,
					CreatedBy = "System",
					CreatedAt = DateTimeOffset.Now
				};

				documentSession.Store(retention);
			}
		}

		private void CheckThroughputInfo(IDocumentSession documentSession)
		{
			var throughputInfo = documentSession.Load<ThroughputInfo>(ThroughputInfo.DocId);
			if (throughputInfo.IsNull())
			{
				throughputInfo = new ThroughputInfo { Id = ThroughputInfo.DocId };
				documentSession.Store(throughputInfo);
			}
		}
	}
}