using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerImplementationEventMethodArgumentRenderer : BaseWithLogging, ILoggerImplementationEventArgumentRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel eventModel, EventArgumentModel model)
        {
            var output = Template.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(Template.Template_ARGUMENT_CLR_TYPE, model.CLRType);

            return output;
        }        
    }
}