using System.Net;
using NLog;
using Newtonsoft.Json;

namespace LogHub.NLog
{
  public class LogHubMessageConvertor
  {
    public string Convert(LogEventInfo logEventInfo, string source)
    {
      var logEventMessage = logEventInfo.FormattedMessage;
      if (logEventMessage == null) return null;

      if (logEventInfo.Exception != null)
      {
        logEventInfo.Properties.Add("ExceptionSource", logEventInfo.Exception.Source);
        logEventInfo.Properties.Add("ExceptionMessage", logEventInfo.Exception.Message);
        logEventInfo.Properties.Add("StackTrace", logEventInfo.Exception.StackTrace);
      }

      logEventInfo.Properties.Add("LoggerName", logEventInfo.LoggerName);

      var logHubMessage = new LogHubMessage
      {
        Host = Dns.GetHostName(),
        Message = logEventMessage,
        TimeStamp = logEventInfo.TimeStamp,
        Level = logEventInfo.Level.GetHashCode(),
        Source = source
      };

      foreach (var property in logEventInfo.Properties)
      {
        var key = property.Key as string;
        var value = property.Value as string;

        if (key == null) continue;
        logHubMessage.Properties.Add(key, value);
      }

      var jsonString = JsonConvert.SerializeObject(logHubMessage);
      return jsonString;
    }
  }
}