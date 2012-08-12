using System;

namespace LogHub.Server.Channels
{
  public interface IChannelListener : IDisposable
  {
    void Listen();
  }
}