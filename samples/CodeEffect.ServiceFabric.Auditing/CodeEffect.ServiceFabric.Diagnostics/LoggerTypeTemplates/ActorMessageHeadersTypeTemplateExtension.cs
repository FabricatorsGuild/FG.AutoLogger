using FG.Diagnostics.AutoLogger.Model;
using FG.Diagnostics.AutoLogger.Model;
using FG.ServiceFabric.Services.Remoting.FabricTransport;

namespace CodeEffect.ServiceFabric.Diagnostics.LoggerTypeTemplates
{
    public class ActorMessageHeadersTypeTemplateExtension : BaseTemplateExtension<CustomServiceRequestHeader>
    {
        private string Definition = @"{
                  ""Name"": ""ActorMessageHeaders"",
                  ""CLRType"": ""FG.ServiceFabric.Actors.Remoting.Runtime.ActorMessageHeaders"",
                  ""Arguments"": [
                    {
                      ""Name"": ""interfaceId"",
                      ""Type"": ""int"",
                      ""Assignment"": ""($this?.InterfaceId ?? 0)""
                    },
                    {
                      ""Name"": ""methodId"",
                      ""Type"": ""int"",
                      ""Assignment"": ""($this?.MethodId ?? 0)""
                    },
                    {
                      ""Name"": ""actorId"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this?.ActorId.ToString()""
                    }
                  ]
                }";
                
        protected override string GetDefinition()
        {
            return Definition;
        }
    }
}