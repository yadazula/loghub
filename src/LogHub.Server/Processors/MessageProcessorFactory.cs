using LogHub.Server.Convertors;

namespace LogHub.Server.Processors
{
  public class MessageProcessorFactory : IMessageProcessorFactory
  {
    private readonly IMessageProcessor rawMessageProcessor;
    private readonly IMessageProcessor chunkedMessageProcessor;

    public MessageProcessorFactory(IMessageProcessor rawMessageProcessor, IMessageProcessor chunkedMessageProcessor)
    {
      this.rawMessageProcessor = rawMessageProcessor;
      this.chunkedMessageProcessor = chunkedMessageProcessor;
    }

    public IMessageProcessor Get(MessageFormat messageFormat)
    {
      return messageFormat == MessageFormat.Chunked ? chunkedMessageProcessor : rawMessageProcessor;
    }
  }
}