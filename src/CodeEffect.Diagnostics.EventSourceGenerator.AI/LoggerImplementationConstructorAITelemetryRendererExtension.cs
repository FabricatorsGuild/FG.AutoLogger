using System.Text;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.AI
{
    public class LoggerImplementationConstructorAITelemetryRendererExtension : AITelemetryRendererExtensionBase, ILoggerImplementationConstructorRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            var output = new StringBuilder();
            output.Append(@"
            _telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();
            _telemetryClient.Context.User.Id = Environment.UserName;
            _telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
");
            return output.ToString();
        }
    }
}