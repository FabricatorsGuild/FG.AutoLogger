using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public class LoggerImplementationMembersAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationMembersRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            return @"private readonly Microsoft.ApplicationInsights.TelemetryClient _telemetryClient;";
        }
    }
}