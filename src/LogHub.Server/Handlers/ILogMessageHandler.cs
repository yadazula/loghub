using LogHub.Server.Core;

namespace LogHub.Server.Handlers
{
  public interface ILogMessageHandler
  {
    bool Handle(LogMessage logMessage);
  }
}