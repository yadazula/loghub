using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogHub.Core.Models;
using LogHub.Server.Convertors;

namespace LogHub.Server.Processors
{
  public class ChunkedMessageProcessor : IMessageProcessor
  {
    private readonly IMessageConvertor<RawMessage, ChunkedMessage> messageConvertor;
    private readonly IMessageProcessor decoratedMessageProcessor;
    private readonly ConcurrentDictionary<string, IList<ChunkedMessage>> messages = new ConcurrentDictionary<string, IList<ChunkedMessage>>();

    public ChunkedMessageProcessor(IMessageConvertor<RawMessage, ChunkedMessage> messageConvertor, IMessageProcessor decoratedMessageProcessor)
    {
      this.messageConvertor = messageConvertor;
      this.decoratedMessageProcessor = decoratedMessageProcessor;
      Task.Factory.StartNew(CheckMessages);
    }

    private void CheckMessages()
    {
      while (true)
      {
        foreach (var message in messages)
        {
          var messageId = message.Key;
          if (IsOutdated(messageId))
          {
            DropMessage(messageId);
            continue;
          }

          if (IsComplete(messageId))
          {
          }
        }
      }
    }

    private bool IsOutdated(string messageId)
    {
      throw new System.NotImplementedException();
    }

    private void DropMessage(string messageId)
    {
      throw new System.NotImplementedException();
    }

    private bool IsComplete(string messageId)
    {
      throw new System.NotImplementedException();
    }

    public void Process(RawMessage rawMessage)
    {
      var chunkedMessage = messageConvertor.Convert(rawMessage);
      if (messages.ContainsKey(chunkedMessage.MessageId))
      {
        var chunkedMessages = messages[chunkedMessage.MessageId];
        chunkedMessages.Add(chunkedMessage);
        return;
      }

      messages.AddOrUpdate(chunkedMessage.MessageId, x => new List<ChunkedMessage> { chunkedMessage },
                           (x, y) =>
                           {
                             y.Add(chunkedMessage);
                             return y;
                           });

    }
  }
}