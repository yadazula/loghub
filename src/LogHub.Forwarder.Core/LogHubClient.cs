using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace LogHub.Forwarder.Core
{
	public class LogHubClient : IDisposable
	{
		private readonly UdpClient udpClient;
		private const int MaxMessageSizeInUdp = 8192;
		private const int MaxMessageSizeInChunk = 8180;
		private const int MaxNumberOfChunksAllowed = 128;

		public LogHubClient()
		{
			udpClient = new UdpClient();
		}

		public void Send(string serverIp, int serverPort, string message)
		{
			var compressedMessage = CompressMessage(message);

			if (compressedMessage.Length <= MaxMessageSizeInUdp)
			{
				udpClient.Send(compressedMessage, compressedMessage.Length, serverIp, serverPort);
				return;
			}
			
			var numberOfChunksRequired = compressedMessage.Length/MaxMessageSizeInChunk + 1;
			if (numberOfChunksRequired > MaxNumberOfChunksAllowed)
			{
				return;
			}

			var messageId = GenerateMessageId(compressedMessage);

			for (var i = 0; i < numberOfChunksRequired; i++)
			{
				var skip = i*MaxMessageSizeInChunk;
				var messageChunkHeader = BuildChunkHeader(messageId, i, numberOfChunksRequired);
				var messageChunkData = compressedMessage.Skip(skip).Take(MaxMessageSizeInChunk).ToArray();

				var messageChunkFull = new byte[messageChunkHeader.Length + messageChunkData.Length];
				messageChunkHeader.CopyTo(messageChunkFull, 0);
				messageChunkData.CopyTo(messageChunkFull, messageChunkHeader.Length);

				udpClient.Send(messageChunkFull, messageChunkFull.Length, serverIp, serverPort);
			}
		}

		private static byte[] BuildChunkHeader(byte[] messageId, int chunkSequenceNumber, int chunkCount)
		{
			var b = new byte[12];

			b[0] = 0x1e;
			b[1] = 0x0f;
			messageId.CopyTo(b, 2);
			b[10] = (byte) chunkSequenceNumber;
			b[11] = (byte) chunkCount;

			return b;
		}

		private static byte[] CompressMessage(String message)
		{
			var compressedMessageStream = new MemoryStream();
			using (var gzipStream = new GZipStream(compressedMessageStream, CompressionMode.Compress))
			{
				var messageBytes = Encoding.UTF8.GetBytes(message);
				gzipStream.Write(messageBytes, 0, messageBytes.Length);
			}

			return compressedMessageStream.ToArray();
		}

		private static byte[] GenerateMessageId(byte[] compressedMessage)
		{
			//create a bit array to store the entire message id (which is 8 bytes)
			var bitArray = new BitArray(64);

			//Read the server ip address
			var ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			var ipAddress =
				(from ip in ipAddresses where ip.AddressFamily == AddressFamily.InterNetwork select ip).FirstOrDefault();

			if (ipAddress == null)
				return null;

			//read bytes of the last 2 segments and insert bits into the bit array
			var addressBytes = ipAddress.GetAddressBytes();
			AddToBitArray(bitArray, 0, addressBytes[2], 0, 8);
			AddToBitArray(bitArray, 8, addressBytes[3], 0, 8);

			//read the current second and insert 6 bits into the bit array
			var second = DateTime.Now.Second;
			AddToBitArray(bitArray, 16, (byte) second, 0, 6);

			//generate the MD5 hash of the compressed message
			byte[] hashOfCompressedMessage;
			using (var md5 = MD5.Create())
			{
				hashOfCompressedMessage = md5.ComputeHash(compressedMessage);
			}

			//insert the first 42 bits into the bit array
			var startIndex = 22;
			for (var hashByteIndex = 0; hashByteIndex < 5; hashByteIndex++)
			{
				var hashByte = hashOfCompressedMessage[hashByteIndex];
				AddToBitArray(bitArray, startIndex, hashByte, 0, 8);
				startIndex += 8;
			}

			//copy all bits from bit array into a byte[]
			var result = new byte[8];
			bitArray.CopyTo(result, 0);

			return result;
		}

		private static void AddToBitArray(BitArray bitArray, int bitArrayIndex, byte byteData, int byteDataIndex, int length)
		{
			var localBitArray = new BitArray(new[] {byteData});

			for (var i = byteDataIndex + length - 1; i >= byteDataIndex; i--)
			{
				bitArray.Set(bitArrayIndex, localBitArray.Get(i));
				bitArrayIndex++;
			}
		}

		public void Dispose()
		{
			((IDisposable) udpClient).Dispose();
		}
	}
}