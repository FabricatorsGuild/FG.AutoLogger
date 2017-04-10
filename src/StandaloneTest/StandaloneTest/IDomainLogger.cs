using System;

namespace StandaloneTest
{
    public interface IDomainLogger
    {
        void StandaloneTestInvoked(string message, int tickcount);
        void StandaloneTestFailed(Exception exception);
        void StandaloneTestRunning(int iteration);
    }
}