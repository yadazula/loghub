using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LogHub.Server.Properties
{
  public class UDPListener
  {
    private readonly int port;
    private readonly UdpClient udpClient;

    public UDPListener(int port)
    {
      this.port = port;
      udpClient = new UdpClient(port);
    }

    public void Start()
    {
      try
      {
        Console.WriteLine("Waiting for broadcast");
        udpClient.BeginReceive(Receive, null);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }
    }

    public void Stop()
    {
      udpClient.Close();
    }

    private void Receive(IAsyncResult ar)
    {
      try
      {
        var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
        var receivedData = udpClient.EndReceive(ar, ref ipEndPoint);
        udpClient.BeginReceive(Receive, null);
        HandleReceivedData(receivedData);
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }
    }

    private void HandleReceivedData(byte[] receivedData)
    {
      Console.WriteLine("Received broadcast : {0}\n", Encoding.ASCII.GetString(receivedData, 0, receivedData.Length));
    }
  }
}