using System;
using System.ServiceProcess;
using NLog;

namespace LogHub.Server.Startup
{
	partial class LogHubService : ServiceBase
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private Bootstrapper bootstrapper;

		public LogHubService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				bootstrapper = new Bootstrapper();
				bootstrapper.Start();
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				throw;
			}
		}

		protected override void OnStop()
		{
			try
			{
				if (bootstrapper != null)
					bootstrapper.Dispose();
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
				throw;
			}
		}
	}
}
