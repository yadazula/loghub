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
      var random = new Random();
      int i = 377;
      while (true)
      {
        var level = random.Next(1, 5);
        switch (level)
        {
          case 1:
            Logger.Debug("message " + (i++));
            break;
          case 2:
            Logger.Info("message " + (i++));
            break;
          case 3:
            Logger.Warn("message " + (i++));
            break;
          case 4:
            Logger.Error("message " + (i++));
            break;
          case 5:
            Logger.Fatal("message " + (i++));
            break;
        }

        Thread.Sleep(50);
      }
    }
  }
}
