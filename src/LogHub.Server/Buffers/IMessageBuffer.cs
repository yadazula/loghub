namespace LogHub.Server.Buffers
{
	public interface IMessageBuffer<in TMessage>
	{
		void Post(TMessage message);
	}
}