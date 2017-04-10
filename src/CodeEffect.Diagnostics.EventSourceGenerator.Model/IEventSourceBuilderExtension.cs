using System.Collections.Generic;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface ILoggerBuilderExtension
    {
        IEnumerable<string> OnEventRendered(string eventName);
    }
}