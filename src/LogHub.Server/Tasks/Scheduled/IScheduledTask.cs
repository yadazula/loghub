using System;

namespace LogHub.Server.Tasks.Scheduled
{
	public interface IScheduledTask
	{
		TimeSpan Period { get; }
		void Execute();
	}
}