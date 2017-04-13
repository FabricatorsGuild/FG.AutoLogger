using System.Collections.Generic;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace ConsoleApplication1
{
    public class LoggerBuilderExtension : IExtension
    {
        public IEnumerable<string> OnEventRendered(string eventName)
        {
            yield return "var a = 1;";
        }
    }
}