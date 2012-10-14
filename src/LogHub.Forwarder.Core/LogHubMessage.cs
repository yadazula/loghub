using System;
using System.Collections.Generic;

namespace LogHub.Forwarder.Core
{
  public class LogHubMessage
  {
    public string Host { get; set; }
    public string Source { get; set; }
    public string Logger { get; set; }
    public int Level { get; set; }
    public string Message { get; set; }
    public DateTimeOffset Date { get; set; }
    public IDictionary<string, object> Properties { get; set; }

    public LogHubMessage()
    {
      Properties = new Dictionary<string, object>();
    }
  }
}