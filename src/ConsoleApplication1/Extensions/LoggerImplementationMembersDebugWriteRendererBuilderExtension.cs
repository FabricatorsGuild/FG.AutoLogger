using FG.Diagnostics.AutoLogger.Model;

namespace ConsoleApplication1.Extensions
{
    public class LoggerImplementationMembersDebugWriteRendererBuilderExtension : ILoggerImplementationMembersRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            return @"// Hello from extension";
        }
    }
}