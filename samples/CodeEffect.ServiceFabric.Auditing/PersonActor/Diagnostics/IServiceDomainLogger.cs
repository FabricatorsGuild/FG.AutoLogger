using System;

namespace PersonActor.Diagnostics
{
	public interface IServiceDomainLogger
    {
		void PersonGenerated(string name, string title);

	    IDisposable RunAsyncLoop();

        void RunAsyncLoopFailed(Exception ex);
    }
}