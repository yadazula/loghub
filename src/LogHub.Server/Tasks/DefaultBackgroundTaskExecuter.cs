using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LogHub.Server.Tasks
{
  public class DefaultBackgroundTaskExecuter : IBackgroundTaskExecuter
  {
    private readonly ConcurrentDictionary<IBackgroundTask, Timer> tasks = new ConcurrentDictionary<IBackgroundTask, Timer>();

    public void Register(IBackgroundTask backgroundTask)
    {
      tasks.GetOrAdd(backgroundTask, x => new Timer(_ => backgroundTask.Execute(), null, TimeSpan.FromSeconds(20), backgroundTask.Period));
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