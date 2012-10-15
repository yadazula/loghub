using System.Web.Http.Dependencies;
using Ninject;

namespace LogHub.Web.Infrastructure.Composition
{
	public class DependencyResolver : DependencyScope, IDependencyResolver
	{
		private readonly IKernel kernel;

		public DependencyResolver(IKernel kernel)
			: base(kernel)
		{
			this.kernel = kernel;
		}

		public IDependencyScope BeginScope()
		{
			return new DependencyScope(kernel);
		}

		public override void Dispose()
		{
			kernel.Dispose();
		}
	}
}