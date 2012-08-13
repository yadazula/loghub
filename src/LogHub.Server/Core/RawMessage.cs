using System;

namespace LogHub.Server.Core
{
  public class RawMessage
  {
    public Guid TrackingId { get; set; }
    public byte[] Payload { get; set; }

    public RawMessage()
    {
      TrackingId = Guid.NewGuid();
    }
  }
}