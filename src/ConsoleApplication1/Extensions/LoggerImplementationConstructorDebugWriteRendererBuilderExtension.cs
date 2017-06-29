using FG.Diagnostics.AutoLogger.Model;

namespace ConsoleApplication1.Extensions
{
    public class LoggerImplementationConstructorDebugWriteRendererBuilderExtension : ILoggerImplementationConstructorRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem)
        {
            return @"// Do stuff in the constructor";
        }

        public string Module => @"Demo";
    }
}