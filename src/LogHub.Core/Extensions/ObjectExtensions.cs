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