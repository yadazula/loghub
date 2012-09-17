using System;
using LogHub.Server.Channels;
using LogHub.Server.Modules;
using Ninject;
using Raven.Client;

namespace LogHub.Server
{
  public class Bootstrapper : IDisposable
  {
    private readonly IKernel kernel;
    private readonly IChannelListener channelListener;
    private readonly IDocumentStore documentStore;

    public Bootstrapper()
    {
      kernel = new StandardKernel();
      kernel.Load<DefaultModule>();
      channelListener = kernel.Get<IChannelListener>();
      documentStore = kernel.Get<IDocumentStore>();
    }

    public void Start()
    {
      channelListener.Listen();
    }

    public void Dispose()
    {
      channelListener.Dispose();
      documentStore.Dispose();
    }
  }
}