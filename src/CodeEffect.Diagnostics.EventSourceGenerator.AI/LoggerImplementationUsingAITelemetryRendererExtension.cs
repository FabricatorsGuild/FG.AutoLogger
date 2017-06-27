using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.AI
{
    public class LoggerImplementationUsingAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationUsingRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            var output = @"using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
";

            return output;
        }
    }
}