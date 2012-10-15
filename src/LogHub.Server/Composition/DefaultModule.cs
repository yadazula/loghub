using System.Collections.Generic;
using System.Configuration;
using LogHub.Core.Indexes;
using LogHub.Core.Models;
using LogHub.Server.Archiving;
using LogHub.Server.Buffers;
using LogHub.Server.Channels;
using LogHub.Server.Convertors;
using LogHub.Server.Handlers;
using LogHub.Server.Processors;
using LogHub.Server.Tasks;
using Ninject;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace LogHub.Server.Composition
{
	public class DefaultModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IDocumentStore>().ToMethod(_ =>
				{
					var store = new DocumentStore {ConnectionStringName = "RavenDB"}.Initialize();
					store.Conventions.SaveEnumsAsIntegers = true;
					IndexCreation.CreateIndexes(typeof (LogMessage_Search).Assembly, store);
					return store;
				}).InSingletonScope();

			Bind<IBufferConsumer<LogMessage>>()
				.To<OutputBufferConsumer>()
				.InSingletonScope();

			Bind<IMessageBuffer<LogMessage>>()
				.To<BatchMessageBuffer<LogMessage>>()
				.InSingletonScope()
				.WithConstructorArgument("batchSize", 4096);

			Bind<ILogMessageHandler>()
				.To<ThroughputCounter>()
				.InSingletonScope()
				.Named("ThroughputCounter");

			Bind<IMessageConvertor<RawMessage, LogMessage>>()
				.To<LogMessageConvertor>()
				.InSingletonScope();

			Bind<IMessageConvertor<RawMessage, ChunkedMessage>>()
				.To<ChunkedMessageConvertor>()
				.InSingletonScope();

			Bind<IMessageConvertor<IList<ChunkedMessage>, RawMessage>>()
				.To<RawMessageConvertor>()
				.InSingletonScope();

			Bind<IMessageProcessor>().ToMethod(c =>
				{
					var throughputCounter = c.Kernel.Get<ILogMessageHandler>("ThroughputCounter");
					var logMessageConvertor = c.Kernel.Get<IMessageConvertor<RawMessage, LogMessage>>();
					var logMessageBuffer = c.Kernel.Get<IMessageBuffer<LogMessage>>();
					return new RawMessageProcessor(logMessageConvertor, logMessageBuffer, throughputCounter);
				})
				.InSingletonScope()
				.Named("RawMessageProcessor");

			Bind<IMessageProcessor>().ToMethod(c =>
				{
					var chunkedMessageConvertor = c.Kernel.Get<IMessageConvertor<RawMessage, ChunkedMessage>>();
					var rawMessageConvertor = c.Kernel.Get<IMessageConvertor<IList<ChunkedMessage>, RawMessage>>();
					var rawMessageProcessor = c.Kernel.Get<IMessageProcessor>("RawMessageProcessor");
					return new ChunkedMessageProcessor(chunkedMessageConvertor, rawMessageConvertor, rawMessageProcessor);
				})
				.InSingletonScope()
				.Named("ChunkedMessageProcessor");

			Bind<IMessageProcessorFactory>().ToMethod(c =>
				{
					var rawMessageProcessor = c.Kernel.Get<IMessageProcessor>("RawMessageProcessor");
					var chunkedMessageProcessor = c.Kernel.Get<IMessageProcessor>("ChunkedMessageProcessor");
					return new MessageProcessorFactory(rawMessageProcessor, chunkedMessageProcessor);
				})
				.InSingletonScope();

			Bind<IBufferConsumer<RawMessage>>()
				.To<InputBufferConsumer>()
				.InSingletonScope();

			Bind<IMessageBuffer<RawMessage>>()
				.To<BatchMessageBuffer<RawMessage>>()
				.InSingletonScope();

			Bind<IChannelListener>()
				.To<UdpChannelListener>()
				.InSingletonScope()
				.WithConstructorArgument("port", int.Parse(ConfigurationManager.AppSettings["UdpListenPort"]));

			Bind<IScheduledTaskExecuter>()
				.To<ScheduledTaskExecuter>()
				.InSingletonScope();

			Bind<ILogArchiver>()
				.To<AmazonGlacierArchiver>()
				.InSingletonScope()
				.Named("AmazonGlacierArchiver");

			Bind<ILogArchiver>()
				.To<AmazonS3Archiver>()
				.InSingletonScope()
				.Named("AmazonS3Archiver");

			Bind<ILogArchiver>()
				.To<DiskArchiver>()
				.InSingletonScope()
				.Named("DiskArchiver");

			Bind<IScheduledTask>()
				.To<RetentionScheduledTask>()
				.InSingletonScope();

			Bind<IScheduledTask>()
				.To<NotificationScheduledTask>()
				.InSingletonScope();
		}
	}
}