using System;
using System.Threading;
using LogHub.Core.Extensions;
using LogHub.Core.Models;
using NLog;
using Raven.Client;

namespace LogHub.Server.Handlers
{
	public class ThroughputHandler : ILogMessageHandler
	{
		private const int Period = 5000;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDocumentStore documentStore;
		private readonly Timer timer;
		private int currentThroughput;
		private int highestThroughput;

		public ThroughputHandler(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
			InitializeHighest();

			timer = new Timer(_ =>
			{
				var current = Thread.VolatileRead(ref currentThroughput);
				Interlocked.Exchange(ref currentThroughput, 0);

				if (current > highestThroughput)
				{
					highestThroughput = current;
				}

				PersistThroughput(current, highestThroughput);
			}, null, Period, Period);
		}

		public string Name
		{
			get { return "Throughput Counter"; }
		}

		public bool Handle(LogMessage logMessage)
		{
			Interlocked.Increment(ref currentThroughput);
			return true;
		}

		private void InitializeHighest()
		{
			using (var documentSession = documentStore.OpenSession())
			{
				var throughputStatistic = documentSession.GetThroughputInfo();
				highestThroughput = throughputStatistic.Highest;
			}
		}

		private void PersistThroughput(int current, int highest)
		{
			try
			{
				using (var documentSession = documentStore.OpenSession())
				{
					var throughputStatistic = new ThroughputInfo
					{
						Id = ThroughputInfo.DocId,
						Highest = highest,
						Current = current
					};

					documentSession.Store(throughputStatistic);
					documentSession.SaveChanges();
				}
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}
		}
	}
}