using System;

namespace LogHub.Core.Models
{
  public class LogRetention
  {
    public string Id { get; set; }
    public string Source { get; set; }
    public ushort Days { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
  }
}