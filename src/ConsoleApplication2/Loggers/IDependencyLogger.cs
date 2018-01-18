using System;

namespace FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1.Loggers
{
    public interface IDependencyLogger
    {
        IDisposable CallExternalComponent(Uri requestName, string content);
        IDisposable RecieveMessage(string message);
    }
}