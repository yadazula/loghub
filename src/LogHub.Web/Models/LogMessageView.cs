namespace LogHub.Web.Models
{
  public class LogMessageView
  {
    public string Host { get; set; }
    public string Source { get; set; }
    public string Logger { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string Date { get; set; }
  }
}