using System.Web.Http;
using System.Web.Http.SelfHost;
using LogHub.Server.Composition;
using Ninject;

namespace LogHub.Server.Channels
{
	public class HttpChannelListener : IChannelListener
	{
		private readonly int port;
		private readonly IKernel kernel;
		private HttpSelfHostConfiguration config;
		private HttpSelfHostServer server;

		public HttpChannelListener(int port, IKernel kernel)
		{
			this.port = port;
			this.kernel = kernel;
		}

		public void Listen()
		{
			config = new HttpSelfHostConfiguration(string.Format("http://127.0.0.1:{0}/", port));
			config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
			config.DependencyResolver = new NinjectDependencyResolver(kernel);

			server = new HttpSelfHostServer(config);
			server.OpenAsync().Wait();
		}

		public void Dispose()
		{
			server.Dispose();
		}
	}
}