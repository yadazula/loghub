using System;

namespace LogHub.Server.Tasks
{
  public interface IBackgroundTaskExecuter : IDisposable
  {
    void Register(IBackgroundTask backgroundTask);
  }
}