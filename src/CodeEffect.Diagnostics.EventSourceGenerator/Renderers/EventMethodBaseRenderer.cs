using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public abstract class EventMethodBaseRenderer : BaseWithLogging
    {
        private static string RenderCLRType(EventArgumentModel model)
        {
            var type = model.CLRType ?? model.Type;
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                    return @"string";
                case ("int"):
                case ("system.int32"):
                    return @"int";
                case ("long"):
                case ("system.int64"):
                    return @"long";
                case ("bool"):
                case ("system.boolean"):
                    return @"bool";
                case ("datetime"):
                case ("system.dateTime"):
                    return @"DateTime";
                case ("guid"):
                case ("system.guid"):
                    return @"Guid";
                default:
                    return type;// @"string";
            }
        }

        protected string RenderWriteEventMethodCallArgument(EventArgumentModel model, bool isPrivateMember = false)
        {
            var output = isPrivateMember ? Template.Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT : Template.Template_METHOD_CALL_PASSTHROUGH_ARGUMENT;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            return output;
        }

        protected string RenderMethodArgument(EventArgumentModel model)
        {
            var output = Template.Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(Template.Template_ARGUMENT_NAME, model.Name);
            output = output.Replace(Template.Template_ARGUMENT_CLR_TYPE, RenderCLRType(model));
            return output;
        }

        protected string RenderAssignment(EventArgumentModel model)
        {
            var output = model.Assignment?.Replace(@"$this", model.Name) ?? model.Name;
            return output;
        }
    }
}