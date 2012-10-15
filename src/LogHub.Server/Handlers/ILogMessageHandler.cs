using LogHub.Core.Models;

namespace LogHub.Server.Handlers
{
	public interface ILogMessageHandler
	{
		string Name { get; }
		bool Handle(LogMessage logMessage);
	}
}