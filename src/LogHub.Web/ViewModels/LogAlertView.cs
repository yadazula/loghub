namespace LogHub.Web.ViewModels
{
	public class LogAlertView
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Host { get; set; }
		public string Source { get; set; }
		public string Level { get; set; }
		public string Message { get; set; }
		public int MessageCount { get; set; }
		public int Minutes { get; set; }
		public string EmailTo { get; set; }
	}
}