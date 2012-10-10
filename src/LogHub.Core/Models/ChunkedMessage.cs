using System;

namespace LogHub.Core.Models
{
  public class ChunkedMessage
  {
    public string MessageId { get; set; }
    public int PartsCount { get; set; }
    public int PartNumber { get; set; }
    public byte[] Data { get; set; }
    public DateTime ArrivalDate { get; set; }
  }
}