using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace LogHub.Core.Models
{
	public class LogMessage
	{
		public string Id { get; set; }
		public string Host { get; set; }
		public string Source { get; set; }
		public string Logger { get; set; }
		public LogLevel Level { get; set; }
		public string Message { get; set; }
		public DateTimeOffset Date { get; set; }
		public IDictionary<string, object> Properties { get; set; }

		[JsonIgnore]
		public string TrackingId { get; set; }

		public bool IsValid()
		{
			return (!string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Message));
		}
	}
}