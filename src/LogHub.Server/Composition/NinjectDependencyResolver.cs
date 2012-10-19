using System.Web.Http.Dependencies;
using Ninject;

namespace LogHub.Server.Composition
{
	public class NinjectDependencyResolver : NinjectScope, IDependencyResolver
	{
		private readonly IKernel kernel;

		public NinjectDependencyResolver(IKernel kernel)
			: base(kernel)
		{
			this.kernel = kernel;
		}

		public IDependencyScope BeginScope()
		{
			return new NinjectScope(kernel);
		}

		public override void Dispose()
		{
		}
	}
}