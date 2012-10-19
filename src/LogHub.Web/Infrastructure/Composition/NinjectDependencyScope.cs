using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Ninject.Parameters;
using Ninject.Syntax;

namespace LogHub.Web.Infrastructure.Composition
{
	public class NinjectDependencyScope : IDependencyScope
	{
		protected IResolutionRoot resolutionRoot;

		public NinjectDependencyScope(IResolutionRoot kernel)
		{
			resolutionRoot = kernel;
		}

		public object GetService(Type serviceType)
		{
			var request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
			return resolutionRoot.Resolve(request).SingleOrDefault();
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			var request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
			return resolutionRoot.Resolve(request).ToList();
		}

		public virtual void Dispose()
		{
		}
	}
}