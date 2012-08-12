using System;
using LogHub.Server.Buffers;
using LogHub.Server.Channels;
using LogHub.Server.Convertors;
using LogHub.Server.Handlers;

namespace LogHub.Server
{
  public class Program
  {
    static void Main()
    {
      ILogMessageHandler throughputCounter = new ThroughputCounter();
      ILogMessageConvertor logMessageConvertor = new LogMessageConvertor();
      IBufferConsumer<byte[]> rawMessageBufferConsumer = new InputBufferConsumer(logMessageConvertor, throughputCounter);
      IMessageBuffer<byte[]> rawMessageBuffer = new BatchMessageBuffer<byte[]>(rawMessageBufferConsumer);

      using (var listener = new UdpChannelListener(11000, rawMessageBuffer))
      {
        listener.Listen();
        Console.WriteLine("Started listening");
        Console.ReadLine();
      }
    }
  }
}