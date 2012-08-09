using System;
using LogHub.Server.Properties;

namespace LogHub.Server
{
  public class Program
  {
    static void Main()
    {
      var listener = new UDPListener(11000);
      listener.Start();
      Console.ReadLine();
      listener.Stop();
    }
  }
}