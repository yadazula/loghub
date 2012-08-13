using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using LogHub.Server.Core;

namespace LogHub.Server.Convertors
{
  public static class MessageFormatExtensions
  {
    private const int HEADER_SIZE = 2;
    private const byte ZLIB_FIRST = 0x78;
    private const byte ZLIB_SECOND = 0x9c;
    private const byte GZIP_FIRST = 0x1f;
    private const byte GZIP_SECOND = 0x8b;
    private const byte UNCOMPRESSED_SECOND = 0x3c;
    private const byte CHUNKED_FIRST = 0x1e;
    private const byte CHUNKED_SECOND = 0x0f;

    public static MessageFormat GetMessageFormat(this RawMessage rawMessage)
    {
      if (rawMessage.Payload.Length < HEADER_SIZE)
      {
        throw new InvalidOperationException("Message is too short. Not even the type header would fit.");
      }

      var first = rawMessage.Payload[0];
      var second = rawMessage.Payload[1];

      if (first == ZLIB_FIRST && second == ZLIB_SECOND)
      {
        return MessageFormat.Deflate;
      }

      if (first == GZIP_FIRST)
      {
        // GZIP and UNCOMPRESSED share first magic byte
        if (second == GZIP_SECOND)
        {
          return MessageFormat.GZip;
        }
        if (second == UNCOMPRESSED_SECOND)
        {
          return MessageFormat.Uncompressed;
        }
      }
      else if (first == CHUNKED_FIRST && second == CHUNKED_SECOND)
      {
        return MessageFormat.Chunked;
      }
      return MessageFormat.Unsupported;
    }

    public static string Decompress(this RawMessage rawMessage, MessageFormat messageFormat)
    {
      var inputStream = messageFormat == MessageFormat.Deflate ?
                                        (Stream)new DeflateStream(new MemoryStream(rawMessage.Payload), CompressionMode.Decompress) :
                                        new GZipStream(new MemoryStream(rawMessage.Payload), CompressionMode.Decompress);

      MemoryStream outputStream;
      using (inputStream)
      {
        var buffer = new byte[rawMessage.Payload.Length];
        outputStream = new MemoryStream();
        int bytesRead;
        while ((bytesRead = inputStream.Read(buffer, 0, rawMessage.Payload.Length)) > 0)
        {
          outputStream.Write(buffer, 0, bytesRead);
        }
      }

      return Encoding.UTF8.GetString(outputStream.ToArray());
    }
  }
}