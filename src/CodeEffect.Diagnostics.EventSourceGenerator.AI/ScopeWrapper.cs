using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public sealed class ScopeWrapper<T> : IDisposable
        where T : OperationTelemetry, new()
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IOperationHolder<T> _operationHolder;
        private readonly Action _onCompleted;

        public ScopeWrapper(TelemetryClient telemetryClient, IOperationHolder<T> operationHolder, Action onCompleted)
        {
            _telemetryClient = telemetryClient;
            _operationHolder = operationHolder;
            _onCompleted = onCompleted;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _onCompleted();
                _telemetryClient.StopOperation<T>(_operationHolder);
            }
        }
    }
}