using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace LogHub.Server.Startup
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		internal const string SERVICE_NAME = "LogHub Server";

		public ProjectInstaller()
		{
			InitializeComponent();

			ServiceName = SERVICE_NAME;

			serviceInstaller1.StartType = ServiceStartMode.Automatic;

			serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
		}

		public string ServiceName
		{
			get
			{
				return serviceInstaller1.DisplayName;
			}
			set
			{
				serviceInstaller1.DisplayName = value;
				serviceInstaller1.ServiceName = value;
			}
		}
	}
}
