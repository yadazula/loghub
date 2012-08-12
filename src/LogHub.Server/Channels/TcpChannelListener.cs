using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LogHub.Server.Buffers;
using NLog;

namespace LogHub.Server.Channels
{
  public class TcpChannelListener : IChannelListener
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly int port;
    private readonly IMessageBuffer<byte[]> messageBuffer;
    private readonly TcpListener tcpListener;

    public TcpChannelListener(int port, IMessageBuffer<byte[]> messageBuffer)
    {
      this.port = port;
      this.messageBuffer = messageBuffer;
      tcpListener = new TcpListener(IPAddress.Any, port);
    }

    public void Listen()
    {
      tcpListener.Start();
      tcpListener.BeginAcceptTcpClient(ReceiveAsync, null);
    }

    private void ReceiveAsync(IAsyncResult ar)
    {
      try
      {
        var tcpClient = tcpListener.EndAcceptTcpClient(ar);
        tcpListener.BeginAcceptTcpClient(ReceiveAsync, null);

        using (tcpClient)
        using (var stream = tcpClient.GetStream())
        {
          var bytes = new Byte[1024];
          int read;
          while ((read = stream.Read(bytes, 0, bytes.Length)) != 0)
          {
            messageBuffer.Post(bytes.Take(read).ToArray());
          }
        }
      }
      catch (Exception exception)
      {
        Logger.Error(exception);
      }
    }

    public void Dispose()
    {
      tcpListener.Stop();
      Logger.Debug("Stopped listening TCP messages on port {0}", port);
    }
  }
}