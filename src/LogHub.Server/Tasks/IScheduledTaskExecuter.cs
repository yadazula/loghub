using System;

namespace LogHub.Server.Tasks
{
	public interface IScheduledTaskExecuter : IDisposable
	{
		void Register(IScheduledTask scheduledTask);
	}
}