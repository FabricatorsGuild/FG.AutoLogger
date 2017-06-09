using System;
using System.Collections.Generic;

namespace WebApiService.Diagnostics
{
	public interface IWebApiLogger
	{
		void ActivatingController(string correlationId, string userId);
	    void StartGetAll();
	    void StopGetAll();


	    IDisposable RecieveWebApiRequest(Uri requestUri, string payload, string correlationId, string userId);
        void RecieveWebApiRequestFailed(Uri requestUri, string payload, string correlationId, string userId, Exception exception);
    }
}