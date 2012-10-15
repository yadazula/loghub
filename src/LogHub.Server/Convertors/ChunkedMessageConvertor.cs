using System;
using System.Linq;
using LogHub.Core.Models;

namespace LogHub.Server.Convertors
{
	public class ChunkedMessageConvertor : IMessageConvertor<RawMessage, ChunkedMessage>
	{
		private const int HEADER_ID_START = 2;
		private const int HEADER_ID_LENGTH = 8;
		private const int HEADER_PART_NUMBER_START = 10;
		private const int HEADER_PART_NUMBER_LENGTH = 1;
		private const int HEADER_PARTS_COUNT_START = 11;
		private const int HEADER_PARTS_COUNT_LENGTH = 1;
		private const int HEADER_TOTAL_LENGTH = 12;

		public ChunkedMessage Convert(RawMessage message)
		{
			var chunkedMessage = new ChunkedMessage
				{
					MessageId = GetMessageId(message.Payload),
					PartNumber = GetPartNumber(message.Payload),
					PartsCount = GetPartsCount(message.Payload),
					Data = GetData(message.Payload),
					ArrivalDate = DateTime.UtcNow
				};

			return chunkedMessage;
		}

		private string GetMessageId(byte[] payload)
		{
			var values = payload.Skip(HEADER_ID_START)
				.Take(HEADER_ID_LENGTH)
				.Select(b => b.ToString("x2"))
				.ToArray();

			var id = string.Concat(values);
			return id;
		}

		private int GetPartNumber(byte[] payload)
		{
			var bytes = payload.Skip(HEADER_PART_NUMBER_START)
				.Take(HEADER_PART_NUMBER_LENGTH)
				.Select(x => System.Convert.ToString(x));

			var partNumberString = string.Concat(bytes);
			var partNumber = System.Convert.ToInt32(partNumberString);
			return partNumber;
		}

		private int GetPartsCount(byte[] payload)
		{
			var bytes = payload.Skip(HEADER_PARTS_COUNT_START)
				.Take(HEADER_PARTS_COUNT_LENGTH)
				.Select(x => System.Convert.ToString(x));

			var partsCountString = string.Concat(bytes);
			var partsCount = System.Convert.ToInt32(partsCountString);
			return partsCount;
		}

		private byte[] GetData(byte[] payload)
		{
			var data = payload.Skip(HEADER_TOTAL_LENGTH).ToArray();
			return data;
		}
	}
}