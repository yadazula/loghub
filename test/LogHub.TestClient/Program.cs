using System;
using System.Threading;
using NLog;

namespace LogHub.TestClient
{
  internal class Program
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static void Main()
    {
      for (int i = 0; i < 1; i++)
      {
        for (var j = 1; j <= 1000; j++)
        {
          Logger.Debug("message " + i);
        }
        Thread.Sleep(500);
      }

      Console.WriteLine("finished sending");
      Console.ReadLine();
    }
  }
}
