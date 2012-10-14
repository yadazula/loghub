using LogHub.Forwarder.Core;
using NLog;
using NLog.Targets;

namespace LogHub.Forwarder.NLog
{
	[Target("LogHub")]
	public class LogHubTarget : TargetWithLayout
	{
		private readonly LogHubMessageConvertor logHubMessageConvertor;
		private readonly LogHubClient logHubClient;

		public string ServerIp { get; set; }
		public int ServerPort { get; set; }
		public string Host { get; set; }
		public string Source { get; set; }

		public LogHubTarget()
		{
			logHubMessageConvertor = new LogHubMessageConvertor();
			logHubClient = new LogHubClient();
		}

		protected override void Write(LogEventInfo logEventInfo)
		{
			var message = Layout.Render(logEventInfo);
			var logHubMessage = logHubMessageConvertor.Convert(logEventInfo, message, Host, Source);
			if (string.IsNullOrWhiteSpace(logHubMessage)) return;
			logHubClient.Send(ServerIp, ServerPort, logHubMessage);
		}

		protected override void CloseTarget()
		{
			logHubClient.Dispose();
			base.CloseTarget();
		}
	}
}