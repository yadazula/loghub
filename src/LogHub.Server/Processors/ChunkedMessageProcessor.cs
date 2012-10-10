using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LogHub.Core.Models;
using LogHub.Server.Convertors;
using NLog;

namespace LogHub.Server.Processors
{
  public class ChunkedMessageProcessor : IMessageProcessor
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IMessageConvertor<RawMessage, ChunkedMessage> chunkedMessageConvertor;
    private readonly IMessageConvertor<IList<ChunkedMessage>, RawMessage> rawMessageConvertor;
    private readonly IMessageProcessor rawMessageProcessor;
    private readonly ConcurrentDictionary<string, IList<ChunkedMessage>> messages = new ConcurrentDictionary<string, IList<ChunkedMessage>>();
    private const int ExpireLimit = 5;

    public ChunkedMessageProcessor(IMessageConvertor<RawMessage, ChunkedMessage> chunkedMessageConvertor,
                                   IMessageConvertor<IList<ChunkedMessage>, RawMessage> rawMessageConvertor,
                                   IMessageProcessor rawMessageProcessor)
    {
      this.chunkedMessageConvertor = chunkedMessageConvertor;
      this.rawMessageConvertor = rawMessageConvertor;
      this.rawMessageProcessor = rawMessageProcessor;
      CheckMessages();
    }

    public void Process(RawMessage rawMessage)
    {
      var chunkedMessage = chunkedMessageConvertor.Convert(rawMessage);
      Logger.Debug("Starting to process message [{0}], part {1}", chunkedMessage.MessageId, chunkedMessage.PartNumber);

      if (messages.ContainsKey(chunkedMessage.MessageId))
      {
        var chunkedMessages = messages[chunkedMessage.MessageId];
        chunkedMessages.Add(chunkedMessage);
      }
      else
      {
        messages.AddOrUpdate(chunkedMessage.MessageId,
                           x => new List<ChunkedMessage> { chunkedMessage },
                           (x, y) =>
                           {
                             y.Add(chunkedMessage);
                             return y;
                           });
      }

      Logger.Debug("Finished processing message [{0}], part {0}", chunkedMessage.MessageId, chunkedMessage.PartNumber);
    }

    private void CheckMessages()
    {
      Task.Factory.StartNew(() =>
      {
        while (true)
        {
          foreach (var message in messages)
          {
            var messageId = message.Key;
            if (IsOutdated(messageId))
            {
              Logger.Debug("Message [{0}] is outdated", messageId);
              DropMessage(messageId);
              continue;
            }

            if (IsComplete(messageId))
            {
              Logger.Debug("Message [{0}] is complete", messageId);
              var logMessage = rawMessageConvertor.Convert(message.Value);
              rawMessageProcessor.Process(logMessage);
              DropMessage(messageId);
            }
          }

          Thread.Sleep(1000);
        }
      }, TaskCreationOptions.LongRunning);
    }

    private bool IsOutdated(string messageId)
    {
      if (!messages.ContainsKey(messageId))
      {
        Logger.Debug("Message [{0}] is not exist in chunk dictionary, not checking if outdated", messageId);
        return false;
      }

      var chunkedMessages = messages[messageId];
      foreach (var chunkedMessage in chunkedMessages)
      {
        var timeSpan = DateTime.UtcNow - chunkedMessage.ArrivalDate;
        if (timeSpan.TotalSeconds > ExpireLimit)
        {
          return true;
        }
      }

      return false;
    }

    private void DropMessage(string messageId)
    {
      IList<ChunkedMessage> list;
      if (messages.TryRemove(messageId, out list))
      {
        Logger.Debug("Dropped message [{0}]", messageId);
        return;
      }

      Logger.Debug("Couldn't drop message [{0}]", messageId);
    }

    private bool IsComplete(string messageId)
    {
      IList<ChunkedMessage> list;
      if (messages.TryGetValue(messageId, out list))
      {
        return list.Count > 0 && list.Count == list[0].PartsCount;
      }

      Logger.Debug("Message [{0}] is not exist in chunk dictionary, not checking if complete", messageId);
      return false;
    }
  }
}