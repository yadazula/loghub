using System;
using System.Collections.Generic;

namespace LogHub.Core.Models
{
	public class MessageCount
	{
		public DateTimeOffset Date { get; set; }
		public int Total { get; set; }
		public IDictionary<string, int> Host { get; set; }
		public IDictionary<string, int> Source { get; set; }

		public MessageCount()
		{
			Host = new Dictionary<string, int>();
			Source = new Dictionary<string, int>();
		}
	}
}