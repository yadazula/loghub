using LogHub.Server.Core;

namespace LogHub.Server.Processors
{
  public interface IMessageProcessor
  {
    void Process(RawMessage rawMessage);
  }
}