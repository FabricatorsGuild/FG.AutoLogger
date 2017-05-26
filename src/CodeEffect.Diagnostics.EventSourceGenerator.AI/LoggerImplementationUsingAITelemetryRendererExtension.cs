using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public class LoggerImplementationUsingAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationUsingRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            var output = @"using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using CodeEffect.Diagnostics.EventSourceGenerator.AI;
";

            return output;
        }
    }
}