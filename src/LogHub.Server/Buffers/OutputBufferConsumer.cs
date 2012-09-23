using LogHub.Core.Models;
using NLog;
using Raven.Client;

namespace LogHub.Server.Buffers
{
  public class OutputBufferConsumer : IBufferConsumer<LogMessage>
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IDocumentStore documentStore;

    public OutputBufferConsumer(IDocumentStore documentStore)
    {
      this.documentStore = documentStore;
    }

    public void Consume(LogMessage[] logMessages)
    {
      Logger.Debug("Saving {0} messages to ravendb.", logMessages.Length);

      using (var session = documentStore.OpenSession())
      {
        foreach (var logMessage in logMessages)
        {
          Logger.Debug("Storing message [{0}].", logMessage.TrackingId);
          session.Store(logMessage);
        }
        session.SaveChanges();
      }

      Logger.Debug("Saved {0} messages to ravendb.", logMessages.Length);
    }
  }
}