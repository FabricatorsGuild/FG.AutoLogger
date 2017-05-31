using System;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics.LoggerTypeTemplates
{
    public class CustomServiceRequestHeaderTypeTemplateExtension : BaseTemplateExtension<CustomServiceRequestHeader>
    {
        private string Definition = @"{
                  ""Name"": ""CustomServiceRequestHeader"",
                  ""CLRType"": ""CodeEffect.ServiceFabric.Services.Remoting.FabricTransport.CustomServiceRequestHeader"",
                  ""Arguments"": [
                    {
                      ""Name"": ""user"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this?.GetHeader(\""name\"")""
                    },
                    {
                      ""Name"": ""correlationId"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this?.GetHeader(\""correlation-id\"")""
                    }
                  ]
                }";
                
        protected override string GetDefinition()
        {
            return Definition;
        }
    }
}