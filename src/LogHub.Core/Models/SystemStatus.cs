using System;

namespace LogHub.Core.Models
{
	public class SystemStatus
	{
		public DatabaseInfo Database { get; set; }
		public ServerInfo Server { get; set; }

		public SystemStatus()
		{
			Database = new DatabaseInfo();
			Server = new ServerInfo();
		}

		public class DatabaseInfo
		{
			public string Size { set; get; }
			public string Version { get; set; }
			public long CountOfDocuments { get; set; }
			public int CountOfIndexes { get; set; }
			public ConnectionState Status { get; set; }
		}

		public class ServerInfo
		{
			public string Version { get; set; }
			public DateTimeOffset? StartTime { get; set; }
			public ConnectionState Status { get; set; }
			public int CurrentThroughput { get; set; }
			public int HighestThroughput { get; set; }
		}

		public enum ConnectionState
		{
			Unknown,
			Online,
			Offline
		}
	}
}