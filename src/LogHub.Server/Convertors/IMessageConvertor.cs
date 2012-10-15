namespace LogHub.Server.Convertors
{
	public interface IMessageConvertor<in TInput, out TOutput>
	{
		TOutput Convert(TInput input);
	}
}