using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerImplementationRenderer : BaseWithLogging, ILoggerImplementationRenderer
    {
        private  string RenderPrivateDeclaration(EventArgumentModel model)
        {
            var output = Template.Template_PRIVATE_MEMBER_DECLARATION;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(Template.Template_ARGUMENT_CLR_TYPE, model.CLRType);

            return output;
        }

        public string RenderPrivateAssignment(EventArgumentModel model)
        {
            var output = Template.Template_PRIVATE_MEMBER_ASSIGNMENT;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);

            return output;
        }

        public virtual string RenderMethodArgument(EventArgumentModel model)
        {
            var output = Template.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(Template.Template_ARGUMENT_CLR_TYPE, model.CLRType);

            return output;
        }

        public void Render(Project project, ProjectItem<LoggerModel> model)
        {
            if (model.ItemType != ProjectItemType.LoggerImplementation)
            {
                LogError($"{model.Name} should be of ItemType {nameof(ProjectItemType.LoggerImplementation)} set but found {model.ItemType}");
                return;
            }

            var loggerModel = model.Content;
            if (loggerModel == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }
            var eventSourceProjectItem = model.DependentUpon as ProjectItem<EventSourceModel>;
            var eventSourceModel = eventSourceProjectItem?.Content;
            if (eventSourceModel == null)
            {
                LogError($"{model.Name} should be DependentUpon a ProjectItem<EventSourceModel>, but found {model.DependentUpon?.GetType().Name ?? "null"}");
                return;
            }


            var output = Template.Template_LOGGER_CLASS_DECLARATION;
            output = output.Replace(Template.Variable_LOGGER_SOURCE_FILE_NAME, loggerModel.SourceFileName);
            output = output.Replace(Template.Variable_NAMESPACE_DECLARATION, loggerModel.LoggerNamespace);
            output = output.Replace(Template.Variable_LOGGER_NAME, loggerModel.Name);
            output = output.Replace(Template.Variable_LOGGER_CLASS_NAME, loggerModel.ClassName);
            output = output.Replace(Template.Variable_EVENTSOURCE_NAMESPACE, eventSourceModel.Namespace);

            var memberDeclarations = new EventArgumentsListBuilder(
                RenderPrivateDeclaration, Template.Template_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION_DELIMITER);
            var constructorMemberAssignments = new EventArgumentsListBuilder(
                RenderPrivateAssignment, Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT);
            var constructorArguments = new EventArgumentsListBuilder(
                RenderMethodArgument, Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION);
            foreach (var argument in loggerModel.ImplicitArguments)
            {
                memberDeclarations.Append(argument);
                constructorArguments.Append(argument);
                constructorMemberAssignments.Append(argument);
            }

            output = output.Replace(Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION, memberDeclarations.ToString());
            output = output.Replace(Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT, constructorMemberAssignments.ToString());
            output = output.Replace(Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION, constructorArguments.ToString());




            var logger = new StringBuilder();
            var loggerEventRenderers = new ILoggerImplementationEventRenderer[]
            {
                new LoggerImplementationEventMethodRenderer(), 
                //new LoggerEventSourcePartialEventMethodRenderer(),
                //new LoggerEventSourcePartialNonEventMethodRenderer(),
            }.Union(project.GetExtensions<ILoggerImplementationEventRenderer>()).ToArray();

            foreach (var loggerEvent in loggerModel.Events)
            {
                foreach (var renderer in loggerEventRenderers)
                {
                    var eventRender = renderer.Render(project, model, loggerEvent);
                    logger.AppendLine(eventRender);
                }
            }

            output = output.Replace(Template.Variable_LOGGER_IMPLEMENTATION, logger.ToString());

            model.Output = output;
        }
    }
}