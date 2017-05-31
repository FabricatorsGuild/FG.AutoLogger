using System;

namespace ConsoleApplication1.Loggers
{
    public interface IDependencyLogger
    {
        IDisposable CallExternalComponent(Uri requestName, string content);

        IDisposable RecieveMessage(string message);
    }
}