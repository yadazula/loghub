using LogHub.Server.Core;
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

    public async void Consume(LogMessage[] logMessages)
    {
      Logger.Debug("Saving {0} messages to ravendb.", logMessages.Length);

      using (var session = documentStore.OpenAsyncSession())
      {
        foreach (var logMessage in logMessages)
        {
          Logger.Debug("Storing message [{0}].", logMessage.TrackingId);
          session.Store(logMessage);
        }
        await session.SaveChangesAsync();
      }

      Logger.Debug("Saved {0} messages to ravendb.", logMessages.Length);
    }
  }
}