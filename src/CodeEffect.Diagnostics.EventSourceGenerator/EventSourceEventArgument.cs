using System;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public class EventSourceEventArgument
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string CLRType { get; set; }

        public TypeTemplateModel TypeTemplate { get; set; }

        [JsonIgnore]
        public EventSourceEventCustomArgument[] ExpandedArguments { get; set; }

        // ReSharper disable InconsistentNaming
        private const string Template_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";
        private const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        private const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";
        private const string Template_PRIVATE_MEMBER_DECLARATION = @"private @@ARGUMENT_CLR_TYPE@@ _@@ARGUMENT_NAME@@;";
        private const string Template_PRIVATE_MEMBER_ASSIGNMENT = @"_@@ARGUMENT_NAME@@ = @@ARGUMENT_NAME@@;";

        private const string Template_METHOD_CALL_PASSTHROUGH_ARGUMENT = @"@@ARGUMENT_NAME@@";
        private const string Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT = @"_@@ARGUMENT_NAME@@";
        // ReSharper restore InconsistentNaming

        

        public virtual void SetCLRType(EventSourcePrototype eventSource)
        {
            var type = this.Type;
            if (this.IsComplexType())
            {
                var template = eventSource.TypeTemplates.FirstOrDefault(t =>
                    t.Name.Equals(this.Type, StringComparison.InvariantCultureIgnoreCase) ||
                    t.CLRType.Equals(this.Type, StringComparison.InvariantCultureIgnoreCase));
                if (template != null)
                {
                    var parsedType = ParseType(template.CLRType);
                    var renderedType = RenderCLRType(parsedType);
                    if (renderedType == "string" && (parsedType != typeof(string)))
                    {
                        type = template.CLRType;
                    }
                    else
                    {
                        type = renderedType;
                    }                    
                }
            }
            this.CLRType = type;
        }

        private Type ParseType()
        {
            return ParseType(this.Type);
        }

        private static Type ParseType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                    return typeof(string);
                case ("int"):
                case ("system.int32"):
                    return typeof(int);
                case ("long"):
                case ("system.int64"):
                    return typeof(long);
                case ("bool"):
                case ("system.boolean"):
                    return typeof(bool);
                case ("datetime"):
                case ("system.dateTime"):
                    return typeof(System.DateTime);
                case ("guid"):
                case ("system.guid"):
                    return typeof(Guid);
                default:
                    return typeof(object);
            }
        }

        private string RenderCLRType()
        {
            var type = ParseType();
            return RenderCLRType(type);
        }

        private static string RenderCLRType(Type type)
        {
            if (type == typeof(string))
                return @"string";
            if (type == typeof(int))
                return @"int";
            if (type == typeof(long))
                return @"long";
            if (type == typeof(bool))
                return @"bool";
            if (type == typeof(DateTime))
                return @"DateTime";
            if (type == typeof(Guid))
                return @"Guid";

            return @"string";
        }

        public bool IsComplexType()
        {
            var type = ParseType();
            return type == typeof(object);
        }
        
        public virtual string RenderPrivateDeclaration()
        {
            var output = Template_PRIVATE_MEMBER_DECLARATION;
            output = output.Replace(Template_ARGUMENT_NAME, this.Name);
            output = output.Replace(Template_ARGUMENT_CLR_TYPE, this.CLRType);

            return output;
        }

        public virtual string RenderPrivateAssignment()
        {
            var output = Template_PRIVATE_MEMBER_ASSIGNMENT;
            output = output.Replace(Template_ARGUMENT_NAME, this.Name);

            return output;
        }        

        public virtual string RenderMethodPureArgument()
        {
            var type = this.CLRType;
            var parsedType = ParseType(this.CLRType);
            var renderedType = RenderCLRType(parsedType);
            if (renderedType == "string" && (parsedType != typeof(string)))
            {
                type = this.CLRType;
            }
            else
            {
                type = renderedType;
            }

            return RenderMethodArgument();
        }

        public virtual string RenderMethodArgument(bool useSimpleTypesOnly = false)
        {
            var output = Template_METHOD_ARGUMENT_DECLARATION;
            output = output.Replace(Template_ARGUMENT_NAME, this.Name);

            var clrType = this.CLRType;
            if (useSimpleTypesOnly)
            {
                clrType = RenderCLRType();
            }
            else
            {
                var parsedType = ParseType(this.CLRType);
                var renderedType = RenderCLRType(parsedType);
                if (renderedType == "string" && (parsedType != typeof(string)))
                {
                    clrType = this.CLRType;
                }
                else
                {
                    clrType = renderedType;
                }
            }

            output = output.Replace(Template_ARGUMENT_CLR_TYPE, clrType);

            return output;
        }

        public virtual string RenderWriteEventMethodCallArgument(bool isPrivateMember = false)
        {
            var output = isPrivateMember ? Template_METHOD_CALL_PRIVATE_MEMBER_ARGUMENT : Template_METHOD_CALL_PASSTHROUGH_ARGUMENT;
            output = output.Replace(Template_ARGUMENT_NAME, this.Name);
            return output;
        }

        public override string ToString()
        {
            return $"{nameof(EventSourceEventArgument)} {this.Name} {this.Type}/{this.CLRType}";
        }
    }
}