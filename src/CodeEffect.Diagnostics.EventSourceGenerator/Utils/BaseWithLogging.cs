using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Utils
{
    public class BaseWithLogging : IWithLogging
    {
        private Action<string> _logMessage;
        private Action<string> _logWarning;
        private Action<string> _logError;

        public void SetLoggers(IWithLogging nextBuilder)
        {
            nextBuilder.SetLogMessage(_logMessage);
            nextBuilder.SetLogWarning(_logWarning);
            nextBuilder.SetLogError(_logError);
        }

        public void SetLogMessage(Action<string> logMessage)
        {
            _logMessage = logMessage;
        }

        public void SetLogWarning(Action<string> logWarning)
        {
            _logWarning = logWarning;
        }

        public void SetLogError(Action<string> logError)
        {
            _logError = logError;
        }
        protected void LogMessage(string message)
        {
            _logMessage(message);
        }
        protected void LogWarning(string warning)
        {
            _logWarning(warning);
        }
        protected void LogError(string error)
        {
            _logError(error);
        }
    }
}