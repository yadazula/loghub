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
			dynamic nLogLogger = GetNLogLogger();
			nLogLogger.Debug("hello");
			
			dynamic log4NetLogger = GetLog4NetLogger();
			log4NetLogger.Debug("hello");

			Console.ReadLine();
			return;

			var random = new Random();
			int i = 0;
			while (true)
			{
				var level = random.Next(1, 5);
				switch (level)
				{
					case 1:
						nLogLogger.Debug("message " + (i++));
						break;
					case 2:
						nLogLogger.Info("message " + (i++));
						break;
					case 3:
						nLogLogger.Warn("message " + (i++));
						break;
					case 4:
						nLogLogger.Error("message " + (i++));
						break;
					case 5:
						nLogLogger.Fatal("message " + (i++));
						break;
				}
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
