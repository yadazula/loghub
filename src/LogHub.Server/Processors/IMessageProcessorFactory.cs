using LogHub.Server.Convertors;

namespace LogHub.Server.Processors
{
	public interface IMessageProcessorFactory
	{
		IMessageProcessor Get(MessageFormat messageFormat);
	}
}