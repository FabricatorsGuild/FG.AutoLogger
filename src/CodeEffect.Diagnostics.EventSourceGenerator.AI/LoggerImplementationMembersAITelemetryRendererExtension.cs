using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.AI
{
    public class LoggerImplementationMembersAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationMembersRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            return @"private readonly Microsoft.ApplicationInsights.TelemetryClient _telemetryClient;";
        }
    }
}