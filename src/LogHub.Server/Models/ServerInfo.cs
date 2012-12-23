using System;

namespace LogHub.Server.Models
{
	public class ServerInfo
	{
		public DateTimeOffset StartTime { get; set; }

		public string Version { get; set; }
	}
}