using System;

namespace LogHub.Core.Models
{
  public class LogAlert
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Host { get; set; }
    public string Source { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public int MessageCount { get; set; }
    public TimeSpan Minutes { get; set; }
    public string User { get; set; }
  }
}