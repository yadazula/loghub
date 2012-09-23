using System.Threading;
using LogHub.Core.Models;
using NLog;

namespace LogHub.Server.Handlers
{
  public class ThroughputCounter : ILogMessageHandler
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Timer throughputTimer;
    private int throughputCount;

    public ThroughputCounter()
    {
      throughputTimer = new Timer(_ =>
      {
        var current = Thread.VolatileRead(ref throughputCount);
        Interlocked.Exchange(ref throughputCount, 0);
        if (current > 0)
        {
          Logger.Info("Throughput : {0}", current);
        }

      }, null, 5000, 5000);
    }

    public string Name
    {
      get { return "Throughput Counter"; }
    }

    public bool Handle(LogMessage logMessage)
    {
      Interlocked.Increment(ref throughputCount);
      return true;
    }
  }
}