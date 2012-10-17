using System;
using System.Linq;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Tasks.Startup
{
	public class CheckDefaultRetention : IStartupTask
	{
		private readonly IDocumentStore documentStore;

		public CheckDefaultRetention(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
		}

		public void Execute()
		{
			using (var documentSession = documentStore.OpenSession())
			{
				var retention = documentSession.Query<Retention>().FirstOrDefault();
				
				if (retention != null)
					return;

				retention = new Retention
				{
					Days = 60, 
					CreatedBy = "System", 
					CreatedAt = DateTimeOffset.Now
				};

				documentSession.Store(retention);
				documentSession.SaveChanges();
			}
		}
	}
}