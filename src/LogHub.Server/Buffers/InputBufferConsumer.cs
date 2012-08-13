using System.Threading.Tasks;
using LogHub.Server.Convertors;
using LogHub.Server.Core;
using LogHub.Server.Processors;

namespace LogHub.Server.Buffers
{
  public class InputBufferConsumer : IBufferConsumer<RawMessage>
  {
    private readonly IMessageProcessorFactory messageProcessorFactory;

    public InputBufferConsumer(IMessageProcessorFactory messageProcessorFactory)
    {
      this.messageProcessorFactory = messageProcessorFactory;
    }

    public void Consume(RawMessage[] rawMessages)
    {
      Parallel.ForEach(rawMessages, rawMessage =>
      {
        var messageFormat = rawMessage.GetMessageFormat();
        var messageProcessor = messageProcessorFactory.Get(messageFormat);
        messageProcessor.Process(rawMessage);
      });
    }
  }
}