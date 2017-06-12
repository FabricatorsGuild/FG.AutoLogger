using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using TinyIoC;

namespace WebApiService
{
	public class TinyIoCResolver : IDependencyResolver
	{
		private readonly TinyIoCContainer _container;

		public TinyIoCResolver(TinyIoCContainer container)
		{
			_container = container;
		}

		public object GetService(Type serviceType)
		{
			try
			{
				return _container.Resolve(serviceType);
			}
			catch (TinyIoCResolutionException)
			{
				return null;
			}
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _container.ResolveAll(serviceType, true);
		}

		public IDependencyScope BeginScope()
		{
			return new TinyIoCResolver(_container.GetChildContainer());
		}

		public void Dispose()
		{
			// Handle dispose
		}
	}
}