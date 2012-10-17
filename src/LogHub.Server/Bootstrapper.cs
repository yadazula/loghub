using System;
using System.Linq;
using System.Threading.Tasks;
using LogHub.Server.Channels;
using LogHub.Server.Composition;
using LogHub.Server.Tasks;
using LogHub.Server.Tasks.Scheduled;
using LogHub.Server.Tasks.Startup;
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

			ExecuteStartupTasks();

			RegisterScheduledTasks();
		}

		private void ExecuteStartupTasks()
		{
			var types = typeof(Bootstrapper).Assembly.GetTypes();
			var typeStartupTask = typeof(IStartupTask);
			var startupTasks = types.Where(x => x.IsClass && typeStartupTask.IsAssignableFrom(x));
			foreach (var startupTask in startupTasks)
			{
				var task = (IStartupTask)kernel.Get(startupTask);
				task.Execute();
			}
		}

		private void RegisterScheduledTasks()
		{
			var types = typeof(Bootstrapper).Assembly.GetTypes();
			var typeStartupTask = typeof(IScheduledTask);
			var scheduledTasks = types.Where(x => x.IsClass && typeStartupTask.IsAssignableFrom(x));
			foreach (var scheduledTask in scheduledTasks)
			{
				var task = (IScheduledTask)kernel.Get(scheduledTask);
				scheduledTaskExecuter.Register(task);
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