using LogHub.Core.Models;
using LogHub.Server.Buffers;
using LogHub.Server.Convertors;
using LogHub.Server.Handlers;
using NLog;

namespace LogHub.Server.Processors
{
  public class SingleMessageProcessor : IMessageProcessor
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IMessageConvertor<RawMessage, LogMessage> messageConvertor;
    private readonly IMessageBuffer<LogMessage> logMessageBuffer;
    private readonly ILogMessageHandler[] handlers;

    public SingleMessageProcessor(IMessageConvertor<RawMessage, LogMessage> messageConvertor, IMessageBuffer<LogMessage> logMessageBuffer, params ILogMessageHandler[] handlers)
    {
      this.messageConvertor = messageConvertor;
      this.logMessageBuffer = logMessageBuffer;
      this.handlers = handlers;
    }

    public void Process(RawMessage rawMessage)
    {
      Logger.Debug("Starting to process message [{0}]", rawMessage.TrackingId);
      var logMessage = messageConvertor.Convert(rawMessage);

      if (!logMessage.IsValid())
      {
        Logger.Debug("Skipping incomplete message [{0}]", logMessage.TrackingId);
        return;
      }

      foreach (var handler in handlers)
      {
        Logger.Debug("Applying handler [{0}] on message [{1}]", handler.Name, logMessage.TrackingId);
        var discard = !handler.Handle(logMessage);

        if (discard)
        {
          Logger.Debug("Handler [{0}] marked message [{1}] to be discarded. Skipping message.", handler.Name, logMessage.TrackingId);
          return;
        }
      }

      Logger.Debug("Finished processing message [{0}]. Posting to buffer.", logMessage.TrackingId);
      logMessageBuffer.Post(logMessage);
    }
  }
}