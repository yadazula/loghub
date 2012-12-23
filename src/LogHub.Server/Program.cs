using System;
using System.Configuration;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using NDesk.Options;
using LogHub.Server.Startup;

namespace LogHub.Server
{
	public class Program
	{
		static void Main(string[] args)
		{
			if (Environment.UserInteractive)
			{
				RunInConsoleMode(args);
				return;
			}

			RunInServiceMode();
		}

		private static void RunInConsoleMode(string[] args)
		{
			Action actionToTake = null;
			var optionSet = new OptionSet
      {
        {"install", "Installs the loghub service", key => actionToTake = InstallService},
        {"uninstall", "Uninstalls the loghub service", key => actionToTake = UninstallService},
        {"start", "Starts the loghub servce", key => actionToTake = StartService},
        {"restart", "Restarts the loghub service", key => actionToTake = RestartService},
        {"stop", "Stops the loghub service", key => actionToTake = StopService},
        {"debug", "Runs loghub service in debug mode", key => actionToTake = RunInDebugMode}
      };

			try
			{
				optionSet.Parse(args);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				PrintOptions(optionSet);
			}

			if (actionToTake == null)
				actionToTake = () => PrintOptions(optionSet);

			actionToTake();
		}

		private static void RunInServiceMode()
		{
			ServiceBase.Run(new LogHubService());
		}

		private static void PrintOptions(OptionSet optionSet)
		{
			Console.WriteLine(@"Command line options :");
			optionSet.WriteOptionDescriptions(Console.Out);
		}

		private static void RunInDebugMode()
		{
			using (var bootstrapper = new Bootstrapper())
			{
				bootstrapper.Start();

				Console.WriteLine("Running LogHub service in debug mode..");
				Console.WriteLine("Press <Enter> to stop");
				Console.ReadKey();
			}
		}

		private static void UninstallService()
		{
			if (ServiceIsInstalled() == false)
			{
				Console.WriteLine("Service is not installed");
			}
			else
			{
				var serviceController = new ServiceController(ProjectInstaller.SERVICE_NAME);

				if (serviceController.Status == ServiceControllerStatus.Running)
					serviceController.Stop();

				ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
			}
		}

		private static void StopService()
		{
			var stopController = new ServiceController(ProjectInstaller.SERVICE_NAME);

			if (stopController.Status == ServiceControllerStatus.Running)
			{
				stopController.Stop();
				stopController.WaitForStatus(ServiceControllerStatus.Stopped);
			}
		}

		private static void StartService()
		{
			var serviceController = new ServiceController(ProjectInstaller.SERVICE_NAME);

			if (serviceController.Status != ServiceControllerStatus.Running)
			{
				serviceController.Start();
				serviceController.WaitForStatus(ServiceControllerStatus.Running);
			}
		}

		private static void RestartService()
		{
			var serviceController = new ServiceController(ProjectInstaller.SERVICE_NAME);

			if (serviceController.Status == ServiceControllerStatus.Running)
			{
				serviceController.Stop();
				serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
			}
			if (serviceController.Status != ServiceControllerStatus.Running)
			{
				serviceController.Start();
				serviceController.WaitForStatus(ServiceControllerStatus.Running);
			}

		}

		private static void InstallService()
		{
			if (ServiceIsInstalled())
			{
				Console.WriteLine("Service is already installed");
			}
			else
			{
				ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
				var serviceController = new ServiceController(ProjectInstaller.SERVICE_NAME);
				serviceController.Start();
			}
		}

		private static bool ServiceIsInstalled()
		{
			return (ServiceController.GetServices().Count(s => s.ServiceName == ProjectInstaller.SERVICE_NAME) > 0);
		}
	}
}