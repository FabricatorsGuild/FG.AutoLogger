using System;

namespace FG.Diagnostics.AutoLogger.Model
{
    public interface IWithLogging
    {
        void SetLogMessage(Action<string> logMessage);
        void SetLogWarning(Action<string> logWarning);
        void SetLogError(Action<string> logError);
    }
}