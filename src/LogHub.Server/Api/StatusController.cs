using System.Diagnostics;
using System.Dynamic;
using System.Web.Http;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Api
{
	public class StatusController : ApiController
	{
		private readonly IDocumentStore documentStore;

		public StatusController(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
		}

		public dynamic Get()
		{
			dynamic status = new ExpandoObject();
			status.Version = FileVersionInfo.GetVersionInfo(typeof(StatusController).Assembly.Location).ProductVersion;
			status.StartTime = Bootstrapper.StartTime.ToString("dd.MM.yyyy HH:mm:ss.fff K");

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