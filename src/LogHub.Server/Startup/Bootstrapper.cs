﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LogHub.Server.Api;
using LogHub.Server.Channels;
using LogHub.Server.Composition;
using LogHub.Server.Models;
using LogHub.Server.Tasks.Scheduled;
using LogHub.Server.Tasks.Startup;
using NLog;
using Ninject;
using Raven.Client;

namespace LogHub.Server.Startup
{
	public class Bootstrapper : IDisposable
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly IKernel kernel;
		private readonly IEnumerable<IChannelListener> channelListeners;
		private readonly IDocumentStore documentStore;
		private readonly IScheduledTaskExecuter scheduledTaskExecuter;

		public Bootstrapper()
		{
			ObserveUnhandledTaskExceptions();

			kernel = new StandardKernel();
			kernel.Load<NinjectDependencyLoader>();

			channelListeners = kernel.GetAll<IChannelListener>();
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
			foreach (var channelListener in channelListeners)
			{
				channelListener.Listen();
			}

			RegisterServerInfo();
		}

		private void RegisterServerInfo()
		{
			var serverInfo = new ServerInfo
			{
				StartTime = DateTimeOffset.Now,
				Version = FileVersionInfo.GetVersionInfo(typeof(Bootstrapper).Assembly.Location).ProductVersion
			};

			kernel.Bind<ServerInfo>().ToConstant(serverInfo);
		}

		public void Dispose()
		{
			scheduledTaskExecuter.Dispose();
			
			foreach (var channelListener in channelListeners)
			{
				channelListener.Dispose();
			}

			documentStore.Dispose();

			kernel.Dispose();
		}
	}
}