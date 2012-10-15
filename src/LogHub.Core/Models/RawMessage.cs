using System;

namespace LogHub.Core.Models
{
	public class RawMessage
	{
		public string TrackingId { get; set; }
		public byte[] Payload { get; set; }

		public RawMessage()
		{
			TrackingId = Guid.NewGuid().ToString();
		}
	}
}