using System;
using System.Collections.Concurrent;
using System.Threading;
using NLog;

namespace LogHub.Server.Tasks
{
	public class ScheduledTaskExecuter : IScheduledTaskExecuter
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly ConcurrentDictionary<IScheduledTask, Timer> tasks = new ConcurrentDictionary<IScheduledTask, Timer>();

		public void Register(IScheduledTask scheduledTask)
		{
			tasks.GetOrAdd(scheduledTask, x => new Timer(_ =>
			{
				try
				{
					scheduledTask.Execute();
				}
				catch (Exception e)
				{
					Logger.ErrorException("Could not execute scheduled task", e);
				}

			}, null, scheduledTask.Period, scheduledTask.Period));
		}

		public void Dispose()
		{
			foreach (var task in tasks)
			{
				task.Value.Dispose();
			}
		}
	}
}