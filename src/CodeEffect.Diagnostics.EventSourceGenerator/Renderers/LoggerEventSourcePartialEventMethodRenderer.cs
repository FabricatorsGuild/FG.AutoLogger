using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class LoggerEventSourcePartialEventMethodRenderer : EventSourceEventMethodRenderer, ILoggerEventSourcePartialEventRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel model)
        {
            return this.Render(model);
        }
    }    
}