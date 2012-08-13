using System;

namespace LogHub.Server
{
  public class Program
  {
    static void Main()
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