using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace LogHub.Server.Buffers
{
  public class BatchMessageBuffer<TMessage> : IMessageBuffer<TMessage>
  {
    private readonly IBufferConsumer<TMessage> bufferConsumer;
    private readonly int batchSize;
    private readonly int autoTriggerBatchPeriod;
    private BufferBlock<TMessage> bufferBlock;

    public BatchMessageBuffer(IBufferConsumer<TMessage> bufferConsumer, int batchSize = 512, int autoTriggerBatchPeriod = 1000)
    {
      this.bufferConsumer = bufferConsumer;
      this.batchSize = batchSize;
      this.autoTriggerBatchPeriod = autoTriggerBatchPeriod;
      CreateBufferBlock();
    }

    private void CreateBufferBlock()
    {
      var messageHandlerBlock = new ActionBlock<TMessage[]>(messages => bufferConsumer.Consume(messages));

      var batcherBlock = new BatchBlock<TMessage>(batchSize);
      batcherBlock.LinkTo(messageHandlerBlock);

      var timer = new Timer(_ => batcherBlock.TriggerBatch());
      Func<TMessage, TMessage> autoTriggerBatch = value =>
      {
        timer.Change(autoTriggerBatchPeriod, Timeout.Infinite);
        return value;
      };

      var timingBlock = new TransformBlock<TMessage, TMessage>(autoTriggerBatch);
      timingBlock.LinkTo(batcherBlock);

      bufferBlock = new BufferBlock<TMessage>();
      bufferBlock.LinkTo(timingBlock);
    }

    public void Post(TMessage message)
    {
      bufferBlock.Post(message);
    }
  }
}