using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using NLog;

namespace LogHub.Server.Buffers
{
	public class BatchMessageBuffer<TMessage> : IMessageBuffer<TMessage>
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly IBufferConsumer<TMessage> bufferConsumer;
		private readonly int batchSize;
		private readonly int autoTriggerBatchPeriod;
		private BufferBlock<TMessage> bufferBlock;

		public BatchMessageBuffer(IBufferConsumer<TMessage> bufferConsumer, int batchSize = 100000,
		                          int autoTriggerBatchPeriod = 2000)
		{
			this.bufferConsumer = bufferConsumer;
			this.batchSize = batchSize;
			this.autoTriggerBatchPeriod = autoTriggerBatchPeriod;
			CreateBufferBlock();
		}

		private void CreateBufferBlock()
		{
			var messageHandlerBlock = new ActionBlock<TMessage[]>(messages =>
				{
					try
					{
						bufferConsumer.Consume(messages);
					}
					catch (Exception exception)
					{
						Logger.Error(exception);
					}
				});

			var batcherBlock = new BatchBlock<TMessage>(batchSize);
			batcherBlock.LinkTo(messageHandlerBlock);

			var timer = new Timer(_ => batcherBlock.TriggerBatch())
				.Change(autoTriggerBatchPeriod, autoTriggerBatchPeriod);

			bufferBlock = new BufferBlock<TMessage>();
			bufferBlock.LinkTo(batcherBlock);
		}

		public void Post(TMessage message)
		{
			bufferBlock.Post(message);
		}
	}
}