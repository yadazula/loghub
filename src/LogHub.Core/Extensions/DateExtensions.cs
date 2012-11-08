using System;

namespace LogHub.Core.Extensions
{
	public static class DateExtensions
	{
		private const string Format = "dd.MM.yyyy HH:mm:ss.fff K";

		public static  string ToOffsetString(this DateTime date)
		 {
			 return date.ToString(Format);
		 }

		 public static string ToOffsetString(this DateTimeOffset date)
		 {
			 return date.ToString(Format);
		 }
	}
}