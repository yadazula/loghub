namespace LogHub.Core.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrWhiteSpace(this string @string)
		{
			return string.IsNullOrWhiteSpace(@string);
		}

		public static bool IsNotNullOrWhiteSpace(this string @string)
		{
			return !string.IsNullOrWhiteSpace(@string);
		}
	}
}