using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FG.ServiceFabric.Services.Remoting.FabricTransport;

namespace Common
{
    public class ServiceRequestContextWrapperX : FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestContextWrapper
	{
		public ServiceRequestContextWrapperX() { }
		public ServiceRequestContextWrapperX(string correlationId, string userId)
		{
			if (ServiceRequestContext.Current == null) return;

			ServiceRequestContext.Current[ServiceRequestContextKeys.CorrelationId] = correlationId;
			ServiceRequestContext.Current[ServiceRequestContextKeys.UserId] = userId;
			ServiceRequestContext.Current[ServiceRequestContextKeys.RequestUri] = null;
		}

		public string CorrelationId
		{
			get { return ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId]; }
			set
			{
				if (ServiceRequestContext.Current != null)
				{
					ServiceRequestContext.Current[ServiceRequestContextKeys.CorrelationId] = value;
				}
			}
		}
		public string UserId
		{
			get { return ServiceRequestContext.Current?[ServiceRequestContextKeys.UserId]; }
			set
			{
				if (ServiceRequestContext.Current != null)
				{
					ServiceRequestContext.Current[ServiceRequestContextKeys.UserId] = value;
				}
			}
		}
		public string RequestUri
		{
			get { return ServiceRequestContext.Current?[ServiceRequestContextKeys.RequestUri]; }
			set
			{
				if (ServiceRequestContext.Current != null)
				{
					ServiceRequestContext.Current[ServiceRequestContextKeys.RequestUri] = value;
				}
			}
		}

		public static class ServiceRequestContextKeys
		{
			public const string CorrelationId = "correlationId";
			public const string UserId = "userId";
			public const string RequestUri = "requestUri";
		}

	}
}
