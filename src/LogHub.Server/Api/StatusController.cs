using System.Diagnostics;
using System.Web.Http;
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

		public Status.ServerInfo Get()
		{
			var serverInfo = new Status.ServerInfo();
			serverInfo.Version = FileVersionInfo.GetVersionInfo(typeof (StatusController).Assembly.Location).ProductVersion;
			serverInfo.StartTime = Bootstrapper.StartTime;
			return serverInfo;
		}
	}
}