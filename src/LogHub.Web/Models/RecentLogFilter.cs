namespace LogHub.Web.Models
{
  public class RecentLogFilter
  {
    public string Host { get; set; }
    public string Source { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public ushort MessageCount { get; set; }
  }
}