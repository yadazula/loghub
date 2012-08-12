using System;
using System.Collections.Generic;

namespace LogHub.Server.Core
{
  public class LogMessage
  {
    public string Id { get; set; }
    public string Host { get; set; }
    public string Source { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp {  get; set;}
    public IDictionary<string, object> Properties {  get; set;}
  }
}