using System;
using System.Threading.Tasks;
using LogHub.Server.Convertors;
using LogHub.Server.Handlers;
using NLog;

namespace LogHub.Server.Buffers
{
  public class InputBufferConsumer : IBufferConsumer<byte[]>
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ILogMessageConvertor logMessageConvertor;
    private readonly ILogMessageHandler[] handlers;

    public InputBufferConsumer(ILogMessageConvertor logMessageConvertor, params ILogMessageHandler[] handlers)
    {
      this.logMessageConvertor = logMessageConvertor;
      this.handlers = handlers;
    }

    public void Consume(byte[][] payloads)
    {
      Parallel.ForEach(payloads, payload =>
      {
        try
        {
          var logMessage = logMessageConvertor.Convert(payload);
          foreach (var handler in handlers)
          {
            handler.Handle(logMessage);
          }
        }
        catch (Exception exception)
        {
          Logger.Error(exception);
        }
      });
    }
  }
}