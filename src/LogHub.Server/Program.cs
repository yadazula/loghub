using System;

namespace LogHub.Server
{
	public class Program
	{
		private static void Main()
		{
			using (var bootstrapper = new Bootstrapper())
			{
				bootstrapper.Start();
				Console.WriteLine("LogHub server started.");
				Console.ReadLine();
			}
		}
	}
}