using System.Collections.Generic;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public interface IExtension
    {
        IEnumerable<string> OnEventRendered(string eventName);
    }
}