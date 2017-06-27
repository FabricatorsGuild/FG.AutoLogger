using FG.Diagnostics.AutoLogger.Generator.Templates;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class EventSourceEventMethodRenderBase : EventMethodBaseRenderer
    {
        private static string RenderEventMethodArgument(EventArgumentModel model)
        {
            var output = EventSourceEventMethodTemplate.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(EventSourceEventMethodTemplate.Variable_ARGUMENT_NAME, model.Name);
            output = output.Replace(EventSourceEventMethodTemplate.Variable_ARGUMENT_CLR_TYPE, RenderEventSourceType(model) ?? model.AssignedCLRType ?? @"string");
            return output;
        }

        private static string RenderWriteEventMethodCallArgument(EventArgumentModel model, bool isPrivateMember = false)
        {
            var output = isPrivateMember ? EventSourceEventMethodTemplate.Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT : EventSourceEventMethodTemplate.Template_METHOD_CALL_PASSTHROUGH_ARGUMENT;
            output = output.Replace(EventSourceEventMethodTemplate.Variable_ARGUMENT_NAME, model.Name);
            return output;
        }

        protected string Render(EventModel model)
        {
            var outputEventMethod = EventSourceEventMethodTemplate.Template_EVENT_METHOD;
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_NAME, model.Name);
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_ID, model.Id.ToString());
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_LEVEL, model.Level.ToString());

            var keywords = new ListBuilder("", " | ", "");
            foreach (var keyword in model.Keywords ?? new KeywordModel[] { })
            {
                keywords.Append($"Keywords.{keyword.Name}");
            }
            var keywordsDeclaration = (keywords.Length > 0) ? $", Keywords = {keywords}" : "";
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_KEYWORDS_DECLARATION, keywordsDeclaration);

            var opCode = (string)null;
            if (model.OpCode != null)
            {
                opCode = model.OpCode.ToString();
            }
            var opCodeDeclaration = (opCode != null) ? $", Opcode = EventOpcode.{opCode}" : "";
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_OPCODE_DECLARATION, opCodeDeclaration);

            var eventTask = (string) null;
            if (model.Task != null)
            {
                eventTask = model.Task.Name;
            }
            var eventTaskDeclaration = (eventTask != null) ? $", Task = Tasks.{eventTask}" : "";
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_TASK_DECLARATION, eventTaskDeclaration);

            var eventMethodArgumentsDeclarationBuilder = new EventArgumentsListBuilder(
                RenderEventMethodArgument, EventSourceEventMethodTemplate.Template_EVENT_METHOD_ARGUMENT_DELIMITER);
            var writeEventMethodCallArgument = new EventArgumentsListBuilder(
                (arg) => RenderWriteEventMethodCallArgument(arg), EventSourceEventMethodTemplate.Template_EVENT_METHOD_CALL_ARGUMENT_DELIMITER);
            writeEventMethodCallArgument.Append($"{model.Name}EventId");

            foreach (var argument in model.GetAllArgumentsExpanded())
            {
                eventMethodArgumentsDeclarationBuilder.Append(argument);
                writeEventMethodCallArgument.Append(argument);
            }

            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_METHOD_ACCESS, model.HasComplexArguments ? "private" : "public");
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_METHOD_ARGUMENTS, eventMethodArgumentsDeclarationBuilder.ToString());
            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_WRITEEVENT_CALL_ARGUMENTS, writeEventMethodCallArgument.ToString());

            outputEventMethod = outputEventMethod.Replace(EventSourceEventMethodTemplate.Variable_EVENT_MESSAGE_FORMATTER, model.MessageFormatter);

            return outputEventMethod;
        }
    }
}