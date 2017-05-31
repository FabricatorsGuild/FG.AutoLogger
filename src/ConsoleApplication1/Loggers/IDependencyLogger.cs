using System;

namespace ConsoleApplication1.Loggers
{
    public interface IDependencyLogger
    {
        IDisposable CallExternalComponent(string requestName, string content);

        IDisposable RecieveMessage(string message);
    }
}