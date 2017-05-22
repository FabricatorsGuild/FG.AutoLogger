using System;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Samples
{
    public interface IConsoleRunnerLogger
    {
        void RunnerCreated();
        void RunnerDestroyed();
        void WaitingForKeyPress();
        void KeyPressed(ConsoleKey key);
        void UnsupportedKeyError(Exception ex);
        void StartLoop();
        void StopLoop();
        void RandomIntsGenerated(int[] values);
    }
}