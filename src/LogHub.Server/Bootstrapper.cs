using System;
using LogHub.Server.Channels;
using LogHub.Server.Composition;
using LogHub.Server.Tasks;
using Ninject;
using Raven.Client;

namespace LogHub.Server
{
  public class Bootstrapper : IDisposable
  {
    private readonly IKernel kernel;
    private readonly IChannelListener channelListener;
    private readonly IDocumentStore documentStore;
    private readonly IScheduledTaskExecuter scheduledTaskExecuter;

    public Bootstrapper()
    {
      kernel = new StandardKernel();
      kernel.Load<DefaultModule>();
      channelListener = kernel.Get<IChannelListener>();
      documentStore = kernel.Get<IDocumentStore>();
      scheduledTaskExecuter = kernel.Get<IScheduledTaskExecuter>();
      var backgroundTasks = kernel.GetAll<IScheduledTask>();
      foreach (var backgroundTask in backgroundTasks)
      {
        scheduledTaskExecuter.Register(backgroundTask);
      }
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