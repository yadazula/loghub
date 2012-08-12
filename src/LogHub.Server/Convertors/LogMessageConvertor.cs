using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using LogHub.Server.Core;
using Newtonsoft.Json;

namespace LogHub.Server.Convertors
{
  public class LogMessageConvertor : ILogMessageConvertor
  {
    private const int HEADER_SIZE = 2;
    private const byte ZLIB_FIRST = 0x78;
    private const byte ZLIB_SECOND = 0x9c;
    private const byte GZIP_FIRST = 0x1f;
    private const byte GZIP_SECOND = 0x8b;
    private const byte UNCOMPRESSED_SECOND = 0x3c;
    private const byte CHUNKED_FIRST = 0x1e;
    private const byte CHUNKED_SECOND = 0x0f;

    public LogMessage Convert(byte[] payload)
    {
      var json = GetJson(payload);
      var logMessage = JsonConvert.DeserializeObject<LogMessage>(json);
      return logMessage;
    }

    private string GetJson(byte[] payload)
    {
      var messageFormat = GetMessageFormat(payload);
      switch (messageFormat)
      {
        case MessageFormat.Deflate:
        case MessageFormat.GZip:
          return Decompress(payload, messageFormat);
        case MessageFormat.Uncompressed:
          return Encoding.UTF8.GetString(payload.Skip(2).ToArray());
        case MessageFormat.Chunked:
        case MessageFormat.Unsupported:
        default:
          throw new InvalidOperationException("Unknown message type. Not supported.");
      }
    }

    private MessageFormat GetMessageFormat(byte[] payload)
    {
      if (payload.Length < HEADER_SIZE)
      {
        throw new InvalidOperationException("Message is too short. Not even the type header would fit.");
      }

      var first = payload[0];
      var second = payload[1];

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

    private string Decompress(byte[] compressedData, MessageFormat messageFormat)
    {
      var inputStream = messageFormat == MessageFormat.Deflate ? 
                                    (Stream) new DeflateStream(new MemoryStream(compressedData), CompressionMode.Decompress) : 
                                    new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress);

      MemoryStream outputStream;
      using (inputStream)
      {
        var buffer = new byte[compressedData.Length];
        outputStream = new MemoryStream();
        int bytesRead;
        while ((bytesRead = inputStream.Read(buffer, 0, compressedData.Length)) > 0)
        {
          outputStream.Write(buffer, 0, bytesRead);
        }
      }

      return Encoding.UTF8.GetString(outputStream.ToArray());
    }
  }
}