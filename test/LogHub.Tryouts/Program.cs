using System;
using System.IO;
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
			dynamic logger = GetNLogLogger();
			//dynamic logger = GetLog4NetLogger();

			var random = new Random();
			int i = 0;
			while (true)
			{
				var level = random.Next(1, 5);
				switch (level)
				{
					case 1:
						logger.Debug("message " + (i++));
						break;
					case 2:
						logger.Info("message " + (i++));
						break;
					case 3:
						logger.Warn("message " + (i++));
						break;
					case 4:
						logger.Error("message " + (i++));
						break;
					case 5:
						logger.Fatal("message " + (i++));
						break;
				}

				Console.ReadLine();
				return;
			}
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
