﻿using System;

namespace FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1.Loggers
{
    public interface IConsoleLogger
    {
        void SayHello(string message);
        void Message(string message);
        void Error(Exception exception);
        void SayGoodbye(string goodbye, DateTime nightTime);

        void Specially(Special special);

        void StartHello();
        void StopHello();

        void LogOtherObject(OtherDataCarryingObject data);
    }

    public class Special
    {
        public int A { get; set; }
        public string B { get; set; }
        public DateTime C { get; set; }
    }
}