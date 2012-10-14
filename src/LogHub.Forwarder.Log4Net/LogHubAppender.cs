using LogHub.Forwarder.Core;
using log4net.Appender;
using log4net.Core;

namespace LogHub.Forwarder.Log4Net
{
	public class LogHubAppender : AppenderSkeleton
	{
		private readonly LogHubMessageConvertor logHubMessageConvertor;
		private readonly LogHubClient logHubClient;

		public string ServerIp { get; set; }
		public int ServerPort { get; set; }
		public string Host { get; set; }
		public string Source { get; set; }

		public LogHubAppender()
		{
			logHubMessageConvertor = new LogHubMessageConvertor();
			logHubClient = new LogHubClient();
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			var message = (Layout != null) ? RenderLoggingEvent(loggingEvent) : loggingEvent.MessageObject.ToString();
			var logHubMessage = logHubMessageConvertor.Convert(loggingEvent, message, Host, Source);
			if (string.IsNullOrWhiteSpace(logHubMessage)) return;
			logHubClient.Send(ServerIp, ServerPort, logHubMessage);
		}

		protected override void OnClose()
		{
			logHubClient.Dispose();
			base.OnClose();
		}
	}
}