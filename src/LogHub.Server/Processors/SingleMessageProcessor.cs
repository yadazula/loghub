using System;
using LogHub.Server.Buffers;
using LogHub.Server.Convertors;
using LogHub.Server.Core;
using LogHub.Server.Handlers;
using NLog;

namespace LogHub.Server.Processors
{
  public class SingleMessageProcessor : IMessageProcessor
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ILogMessageConvertor logMessageConvertor;
    private readonly IMessageBuffer<LogMessage> logMessageBuffer;
    private readonly ILogMessageHandler[] handlers;

    public SingleMessageProcessor(ILogMessageConvertor logMessageConvertor, IMessageBuffer<LogMessage> logMessageBuffer, params ILogMessageHandler[] handlers)
    {
      this.logMessageConvertor = logMessageConvertor;
      this.logMessageBuffer = logMessageBuffer;
      this.handlers = handlers;
    }

    public void Process(RawMessage rawMessage)
    {
      try
      {
        Logger.Debug("Starting to process message [{0}].", rawMessage.TrackingId);
        var logMessage = logMessageConvertor.Convert(rawMessage);

        if (!logMessage.IsValid())
        {
          Logger.Debug("Skipping incomplete message [{0}].", logMessage.TrackingId);
          return;
        }

        foreach (var handler in handlers)
        {
          Logger.Debug("Applying handler [{0}] on message [{1}].", handler.Name, logMessage.TrackingId);
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
      catch (Exception exception)
      {
        Logger.ErrorException(string.Format("Exception thrown while processing message [{0}].", rawMessage.TrackingId), exception);
      }
    }
  }
}