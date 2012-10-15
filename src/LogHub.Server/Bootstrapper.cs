using System;
using System.Threading.Tasks;
using LogHub.Server.Channels;
using LogHub.Server.Composition;
using LogHub.Server.Tasks;
using NLog;
using Ninject;
using Raven.Client;

namespace LogHub.Server
{
	public class Bootstrapper : IDisposable
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly IKernel kernel;
		private readonly IChannelListener channelListener;
		private readonly IDocumentStore documentStore;
		private readonly IScheduledTaskExecuter scheduledTaskExecuter;

		public Bootstrapper()
		{
			ObserveUnhandledTaskExceptions();

			kernel = new StandardKernel();
			kernel.Load<DefaultModule>();
			channelListener = kernel.Get<IChannelListener>();
			documentStore = kernel.Get<IDocumentStore>();
			scheduledTaskExecuter = kernel.Get<IScheduledTaskExecuter>();
			var backgroundTasks = kernel.GetAll<IScheduledTask>();
			foreach (var backgroundTask in backgroundTasks)
			{
				scheduledTaskExecuter.Register(backgroundTask);
			}
		}

		private static void ObserveUnhandledTaskExceptions()
		{
			TaskScheduler.UnobservedTaskException += (sender, args) =>
				{
					foreach (var exception in args.Exception.Flatten().InnerExceptions)
					{
						Logger.Error(exception);
					}

					args.SetObserved();
				};
		}

		public void Start()
		{
			channelListener.Listen();
		}

		public void Dispose()
		{
			scheduledTaskExecuter.Dispose();
			channelListener.Dispose();
			documentStore.Dispose();
		}
	}
}