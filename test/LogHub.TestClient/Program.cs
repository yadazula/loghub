using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LogHub.TestClient
{
  class Program
  {
    static void Main(string[] args)
    {
      var udpClient = new UdpClient();
      try
      {
        udpClient.Connect("127.0.0.1", 11000);
        Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");
        udpClient.Send(sendBytes, sendBytes.Length);
        udpClient.Close();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }

      Console.ReadLine();
    }
  }
}
