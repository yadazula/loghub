using System;

namespace LogHub.Server.Tasks.Scheduled
{
	public interface IScheduledTaskExecuter : IDisposable
	{
		void Register(IScheduledTask scheduledTask);
	}
}