using System;
using System.Linq;
using System.Text;
using LogHub.Core.Models;
using Newtonsoft.Json;

namespace LogHub.Server.Convertors
{
  public class LogMessageConvertor : IMessageConvertor<RawMessage, LogMessage>
  {
    public LogMessage Convert(RawMessage rawMessage)
    {
      var json = GetJson(rawMessage);
      var logMessage = JsonConvert.DeserializeObject<LogMessage>(json);
      logMessage.TrackingId = rawMessage.TrackingId;

      return logMessage;
    }

    private string GetJson(RawMessage rawMessage)
    {
      var messageFormat = rawMessage.GetMessageFormat();
      switch (messageFormat)
      {
        case MessageFormat.Deflate:
        case MessageFormat.GZip:
          return rawMessage.Decompress(messageFormat);
        case MessageFormat.Uncompressed:
          return Encoding.UTF8.GetString(rawMessage.Payload.Skip(2).ToArray());
        default:
          throw new InvalidOperationException("Unknown message type. Not supported.");
      }
    }
  }
}