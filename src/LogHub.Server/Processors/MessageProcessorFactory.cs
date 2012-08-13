using LogHub.Server.Convertors;

namespace LogHub.Server.Processors
{
  public class MessageProcessorFactory : IMessageProcessorFactory
  {
    private readonly IMessageProcessor singleMessageProcessor;
    private readonly IMessageProcessor chunkedMessageProcessor;

    public MessageProcessorFactory(IMessageProcessor singleMessageProcessor, IMessageProcessor chunkedMessageProcessor)
    {
      this.singleMessageProcessor = singleMessageProcessor;
      this.chunkedMessageProcessor = chunkedMessageProcessor;
    }

    public IMessageProcessor Get(MessageFormat messageFormat)
    {
      return messageFormat == MessageFormat.Chunked ? chunkedMessageProcessor : singleMessageProcessor;
    }
  }
}