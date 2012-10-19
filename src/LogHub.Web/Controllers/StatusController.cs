using System.Net;
using System.Net.Sockets;
using LogHub.Core.Models;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;

namespace LogHub.Web.Controllers
{
	public class StatusController : AbstractApiController
	{
		public StatusController(IDocumentSession documentSession)
			: base(documentSession)
		{
		}

		public Status Get()
		{
			var status = new Status();

			GetRavenStats(status);

			return status;
		}

		private void GetRavenStats(Status status)
		{
			var documentStore = (DocumentStore)DocumentSession.Advanced.DocumentStore;
			try
			{
				var versionRequestParams = new CreateHttpJsonRequestParams(null, documentStore.Url + "/build/version", "GET", documentStore.Credentials, documentStore.Conventions);
				var version = documentStore.JsonRequestFactory.CreateHttpJsonRequest(versionRequestParams).ReadResponseJson();

				var dbSizeRequestParams = new CreateHttpJsonRequestParams(null, documentStore.Url + "/database/size", "GET", documentStore.Credentials, documentStore.Conventions);
				var dbSize = documentStore.JsonRequestFactory.CreateHttpJsonRequest(dbSizeRequestParams).ReadResponseJson();

				var databaseStatistics = documentStore.DatabaseCommands.GetStatistics();

				status.Database.Version = version.Value<string>("ProductVersion");
				status.Database.Size = dbSize.Value<string>("DatabaseSizeHumane");
				status.Database.CountOfDocuments = databaseStatistics.CountOfDocuments;
				status.Database.CountOfIndexes = databaseStatistics.CountOfIndexes;
				status.Database.Status = Status.Connection.Online;
			}
			catch (WebException e)
			{
				var socketException = e.InnerException as SocketException;
				if (socketException == null)
					throw;

				switch (socketException.SocketErrorCode)
				{
					case SocketError.AddressNotAvailable:
					case SocketError.NetworkDown:
					case SocketError.NetworkUnreachable:
					case SocketError.ConnectionAborted:
					case SocketError.ConnectionReset:
					case SocketError.TimedOut:
					case SocketError.ConnectionRefused:
					case SocketError.HostDown:
					case SocketError.HostUnreachable:
					case SocketError.HostNotFound:
						status.Database.Status = Status.Connection.Offline;
						break;
					default:
						throw;
				}
			}
		}
	}
}