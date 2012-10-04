namespace LogHub.Core.Extensions
{
	public static class ObjectExtensions
	{
		public static bool IsNull(this object @object)
		{
			return @object == null;
		}

		public static bool IsNotNull(this object @object)
		{
			return @object != null;
		}
	}
}