﻿using System;
using System.Net;
using System.Net.Sockets;
using LogHub.Server.Buffers;
using LogHub.Server.Core;
using NLog;

namespace LogHub.Server.Channels
{
  public class UdpChannelListener : IChannelListener
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly int port;
    private readonly IMessageBuffer<RawMessage> messageBuffer;
    private readonly UdpClient udpClient;
    private IPEndPoint ipEndPoint;

    public UdpChannelListener(int port, IMessageBuffer<RawMessage> messageBuffer)
    {
      this.port = port;
      this.messageBuffer = messageBuffer;
      udpClient = new UdpClient(port);
      ipEndPoint = new IPEndPoint(IPAddress.Any, port);
    }

    public void Listen()
    {
      try
      {
        udpClient.BeginReceive(ReceiveAsync, null);
        Logger.Debug("Started listening UDP messages on port {0}", port);
      }
      catch (Exception exception)
      {
        Logger.Error(exception);
      }
    }

    public void Dispose()
    {
      udpClient.Close();
      Logger.Debug("Stopped listening UDP messages on port {0}", port);
    }

    private void ReceiveAsync(IAsyncResult ar)
    {
      byte[] payload;
      try
      {
        payload = udpClient.EndReceive(ar, ref ipEndPoint);
        udpClient.BeginReceive(ReceiveAsync, null);
      }
      catch (Exception)
      {
        return;
      }

      messageBuffer.Post(new RawMessage { Payload = payload });
    }
  }
}