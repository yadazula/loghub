using LogHub.Core.Models;

namespace LogHub.Server.Processors
{
  public interface IMessageProcessor
  {
    void Process(RawMessage rawMessage);
  }
}