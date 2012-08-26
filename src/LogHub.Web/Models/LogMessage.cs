using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogHub.Web.Models
{
  public class LogMessage
  {
    public string Id { get; set; }
    public string Host { get; set; }
    public string Source { get; set; }
    public string Logger { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public DateTimeOffset Date { get; set; }
    public IDictionary<string, object> Properties { get; set; }
  }
}