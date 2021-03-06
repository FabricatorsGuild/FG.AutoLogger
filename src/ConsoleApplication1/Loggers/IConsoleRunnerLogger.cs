﻿using System;

namespace FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1.Loggers
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