using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using LogHub.Core.Models;
using NLog;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;

namespace LogHub.Web.Controllers
{
	public class StatusController : AbstractApiController
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public StatusController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public async Task<SystemStatus> Get()
		{
			var systemStatus = new SystemStatus();
			systemStatus.Database = await GetDatabaseInfo();
			systemStatus.Server = await GetServerInfo();

			return systemStatus;
		}

		private async Task<SystemStatus.DatabaseInfo> GetDatabaseInfo()
		{
			var documentStore = (DocumentStore)DocumentSession.Advanced.DocumentStore;
			try
			{
				var versionRequestParams = new CreateHttpJsonRequestParams(null, documentStore.Url + "/build/version", "GET", documentStore.Credentials, documentStore.Conventions);
				var version = await documentStore.JsonRequestFactory.CreateHttpJsonRequest(versionRequestParams).ReadResponseJsonAsync();

				var dbSizeRequestParams = new CreateHttpJsonRequestParams(null, documentStore.Url + "/database/size", "GET", documentStore.Credentials, documentStore.Conventions);
				var dbSize = await documentStore.JsonRequestFactory.CreateHttpJsonRequest(dbSizeRequestParams).ReadResponseJsonAsync();

				var databaseStatistics = await documentStore.AsyncDatabaseCommands.GetStatisticsAsync();

				var databaseInfo = new SystemStatus.DatabaseInfo();
				databaseInfo.Version = version.Value<string>("ProductVersion");
				databaseInfo.Size = dbSize.Value<string>("DatabaseSizeHumane");
				databaseInfo.CountOfDocuments = databaseStatistics.CountOfDocuments;
				databaseInfo.CountOfIndexes = databaseStatistics.CountOfIndexes;
				databaseInfo.Status = SystemStatus.ConnectionState.Online;
				return databaseInfo;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				var databaseInfo = new SystemStatus.DatabaseInfo { Status = SystemStatus.ConnectionState.Offline };
				return databaseInfo;
			}
		}

		private async Task<SystemStatus.ServerInfo> GetServerInfo()
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var statusUrl = string.Format("{0}/api/status", ConfigurationManager.AppSettings["ServerAddress"]);
					var responseString = await httpClient.GetStringAsync(statusUrl);
					var serverInfo = JsonConvert.DeserializeObject<SystemStatus.ServerInfo>(responseString);
					serverInfo.Status = SystemStatus.ConnectionState.Online;
					return serverInfo;
				}
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				var serverInfo = new SystemStatus.ServerInfo { Status = SystemStatus.ConnectionState.Offline };
				return serverInfo;
			}
		}
	}
}