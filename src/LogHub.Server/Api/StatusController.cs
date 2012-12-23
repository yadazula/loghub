using System.Dynamic;
using System.Web.Http;
using LogHub.Core.Extensions;
using LogHub.Server.Models;
using LogHub.Server.Startup;
using Raven.Client;

namespace LogHub.Server.Api
{
	public class StatusController : ApiController
	{
		private readonly IDocumentStore documentStore;
		private readonly ServerInfo serverInfo;

		public StatusController(IDocumentStore documentStore, ServerInfo serverInfo)
		{
			this.documentStore = documentStore;
			this.serverInfo = serverInfo;
		}

		public dynamic Get()
		{
			dynamic status = new ExpandoObject();
			status.Version = serverInfo.Version;
			status.StartTime = serverInfo.StartTime;

			using (var documentSession = documentStore.OpenSession())
			{
				var throughputInfo = documentSession.GetThroughputInfo();
				status.CurrentThroughput = throughputInfo.Current;
				status.HighestThroughput = throughputInfo.Highest;
			}

			return status;
		}
	}
}