using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace LogHub.TestClient
{
  internal class Program
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static void Main(string[] args)
    {
      for (int i = 0; i < 1; i++)
      {
        GrayLog();
        Thread.Sleep(1000);
      }

      Console.WriteLine("finished sending");
      Console.ReadLine();
      
      //UDP();
      //TCP();
    }
  
    private static void GrayLog()
    {
      for (int i = 0; i < 1; i++)
      {
        Logger.Debug("message " + (i + 1));
      }
    }

    private static void TCP()
    {
      using (var client = new TcpClient("127.0.0.1", 11000))
      using (var stream = client.GetStream())
      {
        for (int i = 0; i < 10000; i++)
        {
          var sendBytes = Encoding.ASCII.GetBytes("message " + (i + 1));
          stream.Write(sendBytes, 0, sendBytes.Length);
        }
      }

      Console.WriteLine("Finished sending..");
      Console.ReadLine();
    }

    private static void UDP()
    {
      Parallel.For(0, 2, x =>
      {
        var udpClient = new UdpClient();
        try
        {
          udpClient.Connect("127.0.0.1", 11000);
          for (int i = 0; i < 1000; i++)
          {
            var sendBytes = Encoding.ASCII.GetBytes(x + " message " + (i + 1));
            udpClient.Send(sendBytes, sendBytes.Length);
          }

          Console.WriteLine(x + " finished sending");
          Console.ReadLine();
          udpClient.Close();
        }
        catch (Exception e)
        {
          Console.WriteLine(e.ToString());
        }
      });
    }
  }
}
