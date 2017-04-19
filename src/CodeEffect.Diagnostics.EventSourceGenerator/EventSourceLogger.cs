using System.Collections.Generic;
using System.Text;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventSourceLogger : LoggerModel
    {
        public string RenderImplementation(EventSourcePrototype eventSource, int index)
        {
            var className = GetImplementationName();

            var output = Template.Template_LOGGER_CLASS_DECLARATION;
            output = output.Replace(Template.Variable_LOGGER_SOURCE_FILE_NAME, this.SourceFileName);
            output = output.Replace(Template.Variable_NAMESPACE_DECLARATION, this.LoggerNamespace);

            output = output.Replace(Template.Variable_LOGGER_NAME, this.Name);
            output = output.Replace(Template.Variable_LOGGER_NAMESPACE, this.LoggerNamespace);
            output = output.Replace(Template.Variable_LOGGER_CLASS_NAME, className);

            var memberDeclarations = new StringBuilder();
            var memberDeclarationsDelimiter = "";

            var memberAssignments = new StringBuilder();
            var memberAssignmentsDelimiter = "";

            var methodArguments = new StringBuilder();
            var methodArgumentsDelimiter = "";

            var next = 0;
            foreach (var argument in this?.ImplicitArguments ?? new EventSourceEventArgument[0])
            {
                argument.SetCLRType(eventSource);

                var methodArgument = argument.RenderMethodArgument();
                methodArguments.Append($"{methodArgumentsDelimiter}{methodArgument}");
                methodArgumentsDelimiter = Template.Template_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION_DELIMITER;

                var memberDeclaration = argument.RenderPrivateDeclaration();
                memberDeclarations.Append($"{memberDeclarationsDelimiter}{memberDeclaration}");
                memberDeclarationsDelimiter = Template.Template_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION_DELIMITER;

                var memberAssignment = argument.RenderPrivateAssignment();
                memberAssignments.Append($"{memberAssignmentsDelimiter}{memberAssignment}");
                memberAssignmentsDelimiter = Template.Template_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT_DELIMITER;

                next++;
            }
            output = output.Replace(Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION, methodArguments.ToString());
            output = output.Replace(Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_DECLARATION, memberDeclarations.ToString());
            output = output.Replace(Template.Variable_LOGGER_IMPLICIT_ARGUMENTS_MEMBER_ASSIGNMENT, memberAssignments.ToString());

            var implementation = new StringBuilder();
            next = index;

            var renderer = new LoggerEventRenderer();
            foreach (var loggerEvent in this.Events)
            {
                if (ImplicitArguments != null && ImplicitArguments.Length > 0)
                {
                    loggerEvent.InsertImplicitArguments(ImplicitArguments);
                }
                if (OverrideArguments != null && OverrideArguments.Length > 0)
                {
                    loggerEvent.OverrideArguments(OverrideArguments);
                }
                loggerEvent.Keywords = new string[] {this.GetKeyword()};
                implementation.AppendLine(renderer.Render(loggerEvent, next, eventSource));
                next += 1;
            }

            output = output.Replace(Template.Variable_LOGGER_IMPLEMENTATION, implementation.ToString());

            return output;

        }

        public string RenderPartial(EventSourcePrototype eventSource,  int index, string fileName)
        {
            var output = Template.Template_LOGGER_PARTIAL_CLASS_DELCARATION;
            output = output.Replace(Template.Variable_EVENTSOURCE_CLASS_NAME, eventSource.ClassName);
            output = output.Replace(Template.Variable_NAMESPACE_DECLARATION, eventSource.Namespace);
            output = output.Replace(Template.Variable_EVENTSOURCE_PARTIAL_FILE_NAME, fileName);
            output = output.Replace(Template.Variable_LOGGER_SOURCE_FILE_NAME, this.Name);
            

            var logger = new StringBuilder();
            var next = index;

            var renderer = new EventRenderer();
            foreach (var loggerEvent in this.Events)
            {
                if (ImplicitArguments != null && ImplicitArguments.Length > 0)
                {
                    loggerEvent.InsertImplicitArguments(ImplicitArguments);
                }
                if (OverrideArguments != null && OverrideArguments.Length > 0)
                {
                    loggerEvent.OverrideArguments(OverrideArguments);
                }
                loggerEvent.Keywords = new string[] { this.GetKeyword() };
                logger.AppendLine(renderer.Render(loggerEvent, next, eventSource));
                next += 1;
            }

            output = output.Replace(Template.Variable_LOGGER_EVENTS_DECLARATION, logger.ToString());

            return output;
        }

        public void AddTemplate(LoggerTemplateModel loggerTemplate)
        {
            var events = new List<EventModel>();
            this.LoggerNamespace = loggerTemplate.Namespace;
            foreach (var templateEvent in loggerTemplate.Events)
            {                
                events.Add(templateEvent);
            }
            this.Events = events.ToArray();
            this.Include = loggerTemplate.Include;
        }

        public override string ToString()
        {
            return $"{nameof(EventSourceLogger)} {this.Name}";
        }
    }
}