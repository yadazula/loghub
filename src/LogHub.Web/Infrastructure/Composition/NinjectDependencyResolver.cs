using System.Web.Http.Dependencies;
using Ninject;

namespace LogHub.Web.Infrastructure.Composition
{
	public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
	{
		private readonly IKernel kernel;

		public NinjectDependencyResolver(IKernel kernel)
			: base(kernel)
		{
			this.kernel = kernel;
		}

		public IDependencyScope BeginScope()
		{
			return new NinjectDependencyScope(kernel);
		}

		public override void Dispose()
		{
			kernel.Dispose();
		}
	}
}