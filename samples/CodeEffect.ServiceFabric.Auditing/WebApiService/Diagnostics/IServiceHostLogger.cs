using System;

namespace WebApiService.Diagnostics
{
	public interface IServiceHostLogger
	{
		void ServiceTypeRegistered(int processId, string serviceTypeName);
		void ServiceHostInitializationFailed(Exception ex);
	}
}