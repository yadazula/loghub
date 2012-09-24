using System;

namespace LogHub.Server.Tasks
{
  public interface IBackgroundTask
  {
    TimeSpan Period { get; }
    void Execute();
  }
}