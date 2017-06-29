using System.Linq;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventArgumentExtensionMethodBuilder : BaseCoreBuilder, IEventArgumentBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, EventArgumentModel model)
        {
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }

            AddKnownExtensions(eventSource, model);           
        }

        private static void AddKnownExtension(EventSourceModel eventSource, string extensionName, string clrType)
        {
            if (!eventSource.Extensions.Any(ext => ext.CLRType == clrType && ext.Type == extensionName))
            {
                eventSource.Extensions.Add(new ExtensionsMethodModel() { CLRType = clrType, Type = extensionName });
            }
        }


        private static void AddKnownExtensions(EventSourceModel eventSource, EventArgumentModel argument)
        {
            if (argument.TypeTemplate != null)
            {
                foreach (var templateArgument in argument.GetTypeTemplateArguments())
                {
                    AddKnownExtensions(eventSource, templateArgument);
                }
            }

            if (argument.Assignment == null) return;
            var templateCLRType = argument.AssignedCLRType ?? argument.TemplatedParentArgument?.CLRType ?? argument?.TemplatedParentArgument?.Type ?? argument.Type;

            if (argument.Assignment.Contains("$this.AsJson()"))
            {
                AddKnownExtension(eventSource, "AsJson", templateCLRType);
                return;
            }

            if (argument.Assignment.Contains("$this.GetReplicaOrInstanceId()"))
            {
                AddKnownExtension(eventSource, "GetReplicaOrInstanceId", templateCLRType);
                return;
            }

            if (argument.Assignment.Contains("$this.GetContentDigest("))
            {
                AddKnownExtension(eventSource, "GetContentDigest", templateCLRType);
                AddKnownExtension(eventSource, "GetMD5Hash", templateCLRType);
                return;
            }
            if (argument.Assignment.Contains("$this.GetMD5Hash("))
            {
                AddKnownExtension(eventSource, "GetMD5Hash", templateCLRType);
                return;
            }
        }
    }
}