using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public sealed class OperationHolder
    {
        public static void StartOperation(IOperationHolder<RequestTelemetry> aiOperationHolder)
        {
            OperationHolder.Current = new OperationHolder() {AIOperationHolder = aiOperationHolder};
        }

        public static IOperationHolder<RequestTelemetry> StopOperation()
        {
            var aiOperationHolder = OperationHolder.Current.AIOperationHolder;
            OperationHolder.Current = null;

            return aiOperationHolder;
        }

        public IOperationHolder<RequestTelemetry> AIOperationHolder { get; set; }

        private static readonly string ContextKey = Guid.NewGuid().ToString();

        public static OperationHolder Current
        {
            get { return (OperationHolder)CallContext.LogicalGetData(ContextKey); }
            internal set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(ContextKey);
                }
                else
                {
                    CallContext.LogicalSetData(ContextKey, value);
                }
            }
        }
    }
}