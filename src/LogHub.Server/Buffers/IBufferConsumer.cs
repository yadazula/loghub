namespace LogHub.Server.Buffers
{
	public interface IBufferConsumer<in T>
	{
		void Consume(T[] messages);
	}
}