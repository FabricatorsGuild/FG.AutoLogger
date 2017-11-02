using System;

namespace WebApiService.Diagnostics
{
	public interface IOwinCommunicationLogger
	{
		void AbortingWebServer();
		void ClosingWebServer();

		void StartingWebServer(string listeningAddress);
		void ListeningOn(string publishAddress);
		void WebServerFailed(Exception ex);
	}
}