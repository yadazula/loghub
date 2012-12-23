﻿using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LogHub.Core.Models;
using LogHub.Web.ViewModels;
using NLog;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Client.Linq;

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

			var offset = DateTimeOffset.Now.AddHours(-2);
			var messageCounts = DocumentSession.Query<MessageCount>().Where(x => x.Date >= offset).ToList();
			systemStatus.MessageCounts = messageCounts.Select(x => new object[] { x.Date.ToString("hh:mm"), x.Total }).ToList();
			systemStatus.MessageCounts.Insert(0, new object[] { "Date", "Total" });
			return systemStatus;
		}

		private async Task<SystemStatus.DatabaseInfo> GetDatabaseInfo()
		{
			var documentStore = (DocumentStore)DocumentSession.Advanced.DocumentStore;
			try
			{
				var versionRequestParams = new CreateHttpJsonRequestParams(null, documentStore.Url + "/build/version", "GET", documentStore.Credentials, documentStore.Conventions);
				var version = await documentStore.JsonRequestFactory.CreateHttpJsonRequest(versionRequestParams).ReadResponseJsonAsync();

				var url = string.Format("{0}/databases/{1}/database/size", documentStore.Url, documentStore.DefaultDatabase);
				var dbSizeRequestParams = new CreateHttpJsonRequestParams(null, url, "GET", documentStore.Credentials, documentStore.Conventions);
				var dbSize = await documentStore.JsonRequestFactory.CreateHttpJsonRequest(dbSizeRequestParams).ReadResponseJsonAsync();

				var databaseStatistics = await documentStore.AsyncDatabaseCommands.GetStatisticsAsync();

				var databaseInfo = new SystemStatus.DatabaseInfo();
				databaseInfo.Version = version.Value<string>("ProductVersion");
				databaseInfo.Size = dbSize.Value<string>("DatabaseSizeHumane");
				databaseInfo.CountOfDocuments = databaseStatistics.CountOfDocuments;
				databaseInfo.CountOfIndexes = databaseStatistics.CountOfIndexes;
				databaseInfo.Status = SystemStatus.ConnectionOnline;
				return databaseInfo;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				var databaseInfo = new SystemStatus.DatabaseInfo { Status = SystemStatus.ConnectionOffline };
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
					serverInfo.Status = SystemStatus.ConnectionOnline;
					return serverInfo;
				}
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				var serverInfo = new SystemStatus.ServerInfo { Status = SystemStatus.ConnectionOffline };
				return serverInfo;
			}
		}
	}
}