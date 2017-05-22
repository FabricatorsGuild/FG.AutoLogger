using System;

namespace StatelessRunnerService
{
    public interface IStatelessRunnerServiceLogger
    {
        void StartRunAsyncLoop(int iteration, Guid correlationId);
        void StopRunAsyncLoop();

        void RunAsyncWorkCompleted(int iteration);

        void Waiting(int milliseconds);
    }
}