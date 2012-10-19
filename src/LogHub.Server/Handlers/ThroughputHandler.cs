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
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDocumentStore documentStore;
		private readonly Timer throughputTimer;
		private int currentThroughput;
		private int highestThroughput;

		public ThroughputHandler(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
			InitializeHighest();

			throughputTimer = new Timer(_ =>
			{
				var current = Thread.VolatileRead(ref currentThroughput);
				Interlocked.Exchange(ref currentThroughput, 0);

				if (current > highestThroughput)
				{
					highestThroughput = current;
				}

				PersistThroughput(current, highestThroughput);
			}, null, 5000, 5000);
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
				var throughputStatistic = documentSession.Load<ThroughputInfo>(ThroughputInfo.DocId);
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