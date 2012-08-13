using LogHub.Server.Core;

namespace LogHub.Server.Handlers
{
  public interface ILogMessageHandler
  {
    string Name { get; }
    bool Handle(LogMessage logMessage);
  }
}