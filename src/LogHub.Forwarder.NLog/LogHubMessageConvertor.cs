using System.Net;
using LogHub.Forwarder.Core;
using NLog;
using Newtonsoft.Json;

namespace LogHub.Forwarder.NLog
{
	public class LogHubMessageConvertor
	{
		public string Convert(LogEventInfo logEventInfo, string message, string host, string source)
		{
			if (message == null) return null;

			if (logEventInfo.Exception != null)
			{
				logEventInfo.Properties.Add("ExceptionSource", logEventInfo.Exception.Source);
				logEventInfo.Properties.Add("ExceptionMessage", logEventInfo.Exception.Message);
				logEventInfo.Properties.Add("StackTrace", logEventInfo.Exception.StackTrace);
			}

			var logHubMessage = new LogHubMessage
				{
					Host = host ?? Dns.GetHostName(),
					Source = source,
					Message = message,
					Level = Convert(logEventInfo.Level),
					Logger = logEventInfo.LoggerName,
					Date = logEventInfo.TimeStamp
				};

			foreach (var property in logEventInfo.Properties)
			{
				var key = property.Key as string;
				if (key == null) continue;

				var value = property.Value as string;
				logHubMessage.Properties.Add(key, value);
			}

			var jsonString = JsonConvert.SerializeObject(logHubMessage);
			return jsonString;
		}

		private static int Convert(LogLevel logEventInfo)
		{
			if (logEventInfo == LogLevel.Trace)
				return 1;

			if (logEventInfo == LogLevel.Debug)
				return 2;

			if (logEventInfo == LogLevel.Info)
				return 3;

			if (logEventInfo == LogLevel.Warn)
				return 4;

			if (logEventInfo == LogLevel.Error)
				return 5;

			if (logEventInfo == LogLevel.Fatal)
				return 6;

			return 0;
		}
	}
}