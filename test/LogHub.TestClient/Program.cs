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
      int i = 377; 
      while(true)
      {
        Logger.Debug("message " + (i++));
        Thread.Sleep(500);
      }
    }
  }
}
