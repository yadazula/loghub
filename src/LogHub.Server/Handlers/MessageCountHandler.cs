using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using LogHub.Core.Models;
using NLog;
using Raven.Client;

namespace LogHub.Server.Handlers
{
	public class MessageCountHandler : ILogMessageHandler
	{
		private const int Period = 60000;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private int totalCount;
		private readonly ConcurrentDictionary<string, int> countByHost = new ConcurrentDictionary<string, int>();
		private readonly ConcurrentDictionary<string, int> countBySource = new ConcurrentDictionary<string, int>();
		private readonly IDocumentStore documentStore;
		private readonly Timer timer;

		public MessageCountHandler(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;

			timer = new Timer(_ =>
			{
				var host = countByHost.ToDictionary(entry => entry.Key, entry => entry.Value);
				countByHost.Clear();

				var source = countBySource.ToDictionary(entry => entry.Key, entry => entry.Value);
				countBySource.Clear();

				var total = Thread.VolatileRead(ref totalCount);
				Interlocked.Exchange(ref totalCount, 0);

				PersistCounts(total, host, source);

			}, null, Period, Period);
		}

		public string Name { get { return "Message Counter"; } }

		public bool Handle(LogMessage logMessage)
		{
			Interlocked.Increment(ref totalCount);
			countByHost.AddOrUpdate(logMessage.Host, x => 1, (x, y) => y + 1);
			countBySource.AddOrUpdate(logMessage.Source, x => 1, (x, y) => y + 1);
			return true;
		}

		private void PersistCounts(int total, Dictionary<string, int> host, Dictionary<string, int> source)
		{
			try
			{
				using (var documentSession = documentStore.OpenSession())
				{
					var messageCount = new MessageCount { Date = DateTimeOffset.Now, Total = total, Host = host, Source = source };
					documentSession.Store(messageCount);
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