using System.Collections.Generic;
using System.Linq;
using LogHub.Core.Models;

namespace LogHub.Server.Convertors
{
	public class RawMessageConvertor : IMessageConvertor<IList<ChunkedMessage>, RawMessage>
	{
		public RawMessage Convert(IList<ChunkedMessage> chunkedMessages)
		{
			var rawMessage = new RawMessage();
			rawMessage.Payload = new byte[chunkedMessages.Sum(x => x.Data.Length)];

			var offset = 0;
			foreach (var chunkedMessage in chunkedMessages.OrderBy(x => x.PartNumber))
			{
				System.Buffer.BlockCopy(chunkedMessage.Data, 0, rawMessage.Payload, offset, chunkedMessage.Data.Length);
				offset += chunkedMessage.Data.Length;
			}

			rawMessage.TrackingId = chunkedMessages[0].MessageId;
			return rawMessage;
		}
	}
}