using System.Collections;
using System.Net;
using LogHub.Forwarder.Core;
using Newtonsoft.Json;
using log4net.Core;

namespace LogHub.Forwarder.Log4Net
{
	public class LogHubMessageConvertor
	{
		public string Convert(LoggingEvent loggingEvent, string message, string host, string source)
		{
			if (message == null) return null;

			if (loggingEvent.ExceptionObject != null)
			{
				loggingEvent.Properties["ExceptionSource"] = loggingEvent.ExceptionObject.Source;
				loggingEvent.Properties["ExceptionMessage"] = loggingEvent.ExceptionObject.Message;
				loggingEvent.Properties["StackTrace"] = loggingEvent.ExceptionObject.StackTrace;
			}

			var logHubMessage = new LogHubMessage
			{
				Host = host ?? Dns.GetHostName(),
				Source = source,
				Message = message,
				Level = Convert(loggingEvent.Level),
				Logger = loggingEvent.LoggerName,
				Date = loggingEvent.TimeStamp
			};

			foreach (DictionaryEntry property in loggingEvent.Properties)
			{
				var key = property.Key as string;
				if (key == null) continue;

				var value = property.Value as string;
				logHubMessage.Properties.Add(key, value);
			}

			var jsonString = JsonConvert.SerializeObject(logHubMessage);
			return jsonString;
		}

		private static int Convert(Level level)
		{
			if (level == Level.Fine ||
					level == Level.Finer ||
					level == Level.Finest ||
					level == Level.Verbose ||
					level == Level.Trace)
				return 1;

			if (level == Level.Debug)
				return 2;

			if (level == Level.Info)
				return 3;

			if (level == Level.Notice ||
					level == Level.Warn)
				return 4;

			if (level == Level.Error)
				return 5;

			if (level == Level.Critical ||
					level == Level.Severe ||
					level == Level.Alert ||
					level == Level.Emergency ||
					level == Level.Fatal)
				return 6;

			return 0;
		}
	}
}