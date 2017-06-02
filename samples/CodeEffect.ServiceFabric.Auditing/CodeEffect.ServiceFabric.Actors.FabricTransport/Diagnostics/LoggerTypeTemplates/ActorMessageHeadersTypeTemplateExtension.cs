using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics.LoggerTypeTemplates
{
    public class ActorMessageHeadersTypeTemplateExtension : BaseTemplateExtension<CustomServiceRequestHeader>
    {
        private string Definition = @"{
                  ""Name"": ""ActorMessageHeaders"",
                  ""CLRType"": ""CodeEffect.ServiceFabric.Actors.Remoting.Runtime.ActorMessageHeaders"",
                  ""Arguments"": [
                    {
                      ""Name"": ""InterfaceId"",
                      ""Type"": ""int"",
                      ""Assignment"": ""($this?.InterfaceId ?? 0)""
                    },
                    {
                      ""Name"": ""MethodId"",
                      ""Type"": ""int"",
                      ""Assignment"": ""($this?.MethodId ?? 0)""
                    },
                    {
                      ""Name"": ""ActorId"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this?.ActorId.ToString()""
                    },
                  ]
                }";
                
        protected override string GetDefinition()
        {
            return Definition;
        }
    }
}