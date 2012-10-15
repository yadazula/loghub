namespace LogHub.Server.Convertors
{
	public enum MessageFormat
	{
		Unsupported,
		Deflate,
		GZip,
		Chunked,
		Uncompressed
	}
}