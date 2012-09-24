using System;
using LogHub.Server.Channels;
using LogHub.Server.Modules;
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
    private readonly IBackgroundTaskExecuter backgroundTaskExecuter;

    public Bootstrapper()
    {
      kernel = new StandardKernel();
      kernel.Load<DefaultModule>();
      channelListener = kernel.Get<IChannelListener>();
      documentStore = kernel.Get<IDocumentStore>();
      backgroundTaskExecuter = kernel.Get<IBackgroundTaskExecuter>();
      var backgroundTasks = kernel.GetAll<IBackgroundTask>();
      foreach (var backgroundTask in backgroundTasks)
      {
        backgroundTaskExecuter.Register(backgroundTask);
      }
    }

    public void Start()
    {
      channelListener.Listen();
    }

    public void Dispose()
    {
      backgroundTaskExecuter.Dispose();
      channelListener.Dispose();
      documentStore.Dispose();
    }
  }
}