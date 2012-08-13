using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogHub.Server.Core
{
  public class LogMessage
  {
    public string Id { get; set; }
    public string Host { get; set; }
    public string Source { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public DateTimeOffset TimeStamp {  get; set;}
    public IDictionary<string, object> Properties {  get; set;}

    [JsonIgnore]
    public Guid TrackingId { get; set; }

    public bool IsValid()
    {
      return (!string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Message));
    }
  }
}