using System.Collections.Generic;

namespace LogHub.Web.ViewModels
{
	public class SystemStatus
	{
		public const string ConnectionOnline = "Online";
		public const string ConnectionOffline = "Offline";

		public DatabaseInfo Database { get; set; }
		public ServerInfo Server { get; set; }
		public IList<object[]> MessageCounts { get; set; }

		public SystemStatus()
		{
			Database = new DatabaseInfo();
			Server = new ServerInfo();
			MessageCounts = new List<object[]>();
		}

		public class DatabaseInfo
		{
			public string Size { set; get; }
			public string Version { get; set; }
			public long CountOfDocuments { get; set; }
			public int CountOfIndexes { get; set; }
			public string Status { get; set; }
		}

		public class ServerInfo
		{
			public string Version { get; set; }
			public string StartTime { get; set; }
			public string Status { get; set; }
			public int CurrentThroughput { get; set; }
			public int HighestThroughput { get; set; }
		}
	}
}