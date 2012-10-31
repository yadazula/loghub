namespace LogHub.Core.Models
{
	public class ThroughputInfo
	{
		public const string DocId = "ThroughputInfos/1";

		public string Id { get; set; }
		public int Current { get; set; }
		public int Highest { get; set; }
	}
}