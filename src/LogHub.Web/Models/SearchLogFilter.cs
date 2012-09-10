namespace LogHub.Web.Models
{
  public class SearchLogFilter
  {
    public string Host { get; set; }
    public string Source { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public ushort MessageCount { get; set; }
    public ushort? Page { get; set; }
  }
}