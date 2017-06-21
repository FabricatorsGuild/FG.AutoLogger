using System.Linq;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Builders;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Templates;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class LoggerImplementationRenderer : BaseWithLogging, ILoggerImplementationRenderer
    {
        private static string RenderPrivateDeclaration(EventArgumentModel model)
        {
            var output = LoggerImplementationTemplate.Template_PRIVATE_MEMBER_DECLARATION;
            output = output.Replace(LoggerImplementationTemplate.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(LoggerImplementationTemplate.Template_ARGUMENT_CLR_TYPE, model.CLRType);

            return output;
        }

        private static string RenderPrivateAssignment(EventArgumentModel model)
        {
            var output = LoggerImplementationTemplate.Template_PRIVATE_MEMBER_ASSIGNMENT;
            output = output.Replace(LoggerImplementationTemplate.Template_ARGUMENT_NAME, model.Name);

            return output;
        }

        private static string RenderMethodArgument(EventArgumentModel model)
        {
            var output = LoggerImplementationTemplate.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(LoggerImplementationTemplate.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(LoggerImplementationTemplate.Template_ARGUMENT_CLR_TYPE, model.CLRType);

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


            var output = LoggerImplementationTemplate.Template_LOGGER_CLASS_DECLARATION;
            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_SOURCE_FILE_NAME, loggerModel.SourceFileName);
            output = output.Replace(LoggerImplementationTemplate.Variable_NAMESPACE_DECLARATION, loggerModel.LoggerNamespace);
            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_NAME, loggerModel.Name);
            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_CLASS_NAME, loggerModel.ClassName);
            output = output.Replace(LoggerImplementationTemplate.Variable_EVENTSOURCE_NAMESPACE, eventSourceModel.Namespace);

            var memberDeclarations = new EventArgumentsListBuilder(
                RenderPrivateDeclaration, LoggerImplementationTemplate.Template_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION_DELIMITER);
            var constructorMemberAssignments = new EventArgumentsListBuilder(
                RenderPrivateAssignment, LoggerImplementationTemplate.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT_DELIMITER);
            var constructorArguments = new EventArgumentsListBuilder(
                RenderMethodArgument, LoggerImplementationTemplate.Variable_LOGGER_IMPLICIT_ARGUMENTS_METHOD_CONSTRUCTOR_DELIMITER);
            foreach (var argument in loggerModel.ImplicitArguments)
            {
                memberDeclarations.Append(argument);
                constructorArguments.Append(argument);
                constructorMemberAssignments.Append(argument);
            }

            var usings = new StringBuilder();
            var usingRenderers = new ILoggerImplementationUsingRenderer[]
            {
            }.Union(project.GetExtensions<ILoggerImplementationUsingRenderer>()).ToArray();
            foreach (var renderer in usingRenderers)
            {
                PassAlongLoggers(renderer as IWithLogging);
                usings.Append(renderer.Render(project, model));
            }

            var memberRenderers = new ILoggerImplementationMembersRenderer[]
            {
            }.Union(project.GetExtensions<ILoggerImplementationMembersRenderer>()).ToArray();
            foreach (var renderer in memberRenderers)
            {
                PassAlongLoggers(renderer as IWithLogging);
                memberDeclarations.Append(renderer.Render(project, model));
            }

            var constructorRenderers = new ILoggerImplementationConstructorRenderer[]
            {
            }.Union(project.GetExtensions<ILoggerImplementationConstructorRenderer>()).ToArray();
            foreach (var renderer in constructorRenderers)
            {
                PassAlongLoggers(renderer as IWithLogging);
                constructorMemberAssignments.Append(renderer.Render(project, model));
            }

            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_IMPLICIT_USING_DECLARATION, usings.ToString());
            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION, memberDeclarations.ToString());
            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT, constructorMemberAssignments.ToString());
            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_IMPLICIT_ARGUMENTS_CONSTRUCTOR_DECLARATION, constructorArguments.ToString());

            var logger = new StringBuilder();
            var loggerEventRenderers = new ILoggerImplementationEventRenderer[]
            {
                new LoggerImplementationEventMethodRenderer(), 
            }.Union(project.GetExtensions<ILoggerImplementationEventRenderer>()).ToArray();

            foreach (var loggerEvent in loggerModel.Events)
            {
                foreach (var renderer in loggerEventRenderers)
                {
                    PassAlongLoggers(renderer as IWithLogging);
                    var eventRender = renderer.Render(project, model, loggerEvent);
                    logger.AppendLine(eventRender);
                }
            }

            output = output.Replace(LoggerImplementationTemplate.Variable_LOGGER_IMPLEMENTATION, logger.ToString());

            model.Output = output;
        }
    }
}