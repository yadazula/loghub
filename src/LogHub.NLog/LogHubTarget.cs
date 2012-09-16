using NLog;
using NLog.Targets;

namespace LogHub.NLog
{
  [Target("LogHub")]
  public class LogHubTarget : TargetWithLayout
  {
    private readonly LogHubMessageConvertor logHubMessageConvertor;
    private readonly LogHubClient logHubClient;

    public string ServerIp { get; set; }
    public int ServerPort { get; set; }
    public string Host { get; set; }
    public string Source { get; set; }

    public LogHubTarget()
    {
      logHubMessageConvertor = new LogHubMessageConvertor();
      logHubClient = new LogHubClient();
    }

    protected override void Write(LogEventInfo logEvent)
    {
      var logHubMessage = logHubMessageConvertor.Convert(logEvent, Host, Source);
      if(string.IsNullOrWhiteSpace(logHubMessage)) return;
      logHubClient.Send(ServerIp, ServerPort, logHubMessage);
    }

    protected override void CloseTarget()
    {
      logHubClient.Dispose();
      base.CloseTarget();
    }
  }
}