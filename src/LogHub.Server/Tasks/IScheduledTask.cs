using System;

namespace LogHub.Server.Tasks
{
  public interface IScheduledTask
  {
    TimeSpan Period { get; }
    void Execute();
  }
}