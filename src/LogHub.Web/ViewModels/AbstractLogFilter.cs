using LogHub.Core.Models;

namespace LogHub.Web.ViewModels
{
	public abstract class AbstractLogFilter
	{
		public string Host { get; set; }
		public string Source { get; set; }
		public LogLevel Level { get; set; }
		public string Message { get; set; }
		public ushort? MessageCount { get; set; }
	}
}