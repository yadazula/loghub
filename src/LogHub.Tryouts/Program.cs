using System;
using System.IO;
using System.Threading;
using ET.FakeText;
using NLog;
using NLog.Config;
using log4net;
using LogManager = NLog.LogManager;

namespace LogHub.Tryouts
{
	public class Program
	{
		private static void Main()
		{
			var textGenerator = new TextGenerator();
			dynamic logger = GetNLogLogger();
			var random = new Random();

			var message = textGenerator.GenerateText(random.Next(50, 150));

			int i = 0;
			while (i < 10)
			{
				for (int j = 0; j < 10; j++)
				{
				var level = random.Next(1, 5);
					switch (level)
					{
						case 1:
							logger.Debug(message);
							break;
						case 2:
							logger.Info(message);
							break;
						case 3:
							logger.Warn(message);
							break;
						case 4:
							logger.Error(message);
							break;
						case 5:
							logger.Fatal(message);
							break;
					}
					i++;
				}
				Console.WriteLine(i);
				Thread.Sleep(200);
			}

			Console.WriteLine("finito");
			Console.ReadLine();
		}

		public static Logger GetNLogLogger()
		{
			LogManager.Configuration = new XmlLoggingConfiguration("NLogConfig.xml");
			var logger = LogManager.GetCurrentClassLogger();
			return logger;
		}

		public static ILog GetLog4NetLogger()
		{
			log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("Log4NetConfig.xml"));
			var logger = log4net.LogManager.GetLogger(typeof(Program));
			return logger;
		}
	}
}