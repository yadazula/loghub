using System;
using LogHub.Core.Models;
using LogHub.Server.Api;
using LogHub.Server.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raven.Client;

namespace LogHub.Tests.Server.Api
{
	[TestClass]
	public class StatusControllerTest
	{
		[TestMethod]
		public void Get_should_return_status_info()
		{
			var throughputInfo = new ThroughputInfo
			{
				Current = 5,
				Highest = 10
			};

			var serverInfo = new ServerInfo
			{
				StartTime = DateTimeOffset.Now,
				Version = "1.0.0"
			};

			var documentSessionMock = new Mock<IDocumentSession>();
			documentSessionMock.Setup(x => x.Load<ThroughputInfo>(ThroughputInfo.DocId)).Returns(throughputInfo);

			var documentStoreMock = new Mock<IDocumentStore>();
			documentStoreMock.Setup(x => x.OpenSession()).Returns(documentSessionMock.Object);

			var statusController = new StatusController(documentStoreMock.Object, serverInfo);
			var statusInfo = statusController.Get();

			Assert.AreEqual(serverInfo.Version, statusInfo.Version);
			Assert.AreEqual(serverInfo.StartTime, statusInfo.StartTime);
			Assert.AreEqual(throughputInfo.Current, statusInfo.CurrentThroughput);
			Assert.AreEqual(throughputInfo.Highest, statusInfo.HighestThroughput);
		}
	}
}