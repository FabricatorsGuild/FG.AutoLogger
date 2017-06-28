using System.Fabric;
using FG.Diagnostics.AutoLogger.Model;
using FG.ServiceFabric.Services.Remoting.FabricTransport;

namespace CodeEffect.ServiceFabric.Diagnostics.LoggerTypeTemplates
{
    public class ServiceRequestContextTypeTemplateExtension : BaseTemplateExtension<ServiceRequestContext>
    {
        private string Definition = @"{
              ""Name"": ""ServiceRequestContext"",
              ""CLRType"": ""FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestContext"",
              ""Arguments"": [
                {
                  ""Assignment"": ""FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestContext.Current?[\""correlationId\""]"",
                  ""Name"": ""correlationId"",
                  ""Type"": ""string""
                },
                {
                  ""Assignment"": ""FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestContext.Current?[\""userId\""]"",
                  ""Name"": ""userId"",
                  ""Type"": ""string""
                },
                {
                  ""Assignment"": ""FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestContext.Current?[\""requestUri\""]"",
                  ""Name"": ""requestUri"",
                  ""Type"": ""string""
                }
              ]
            }";

        protected override string GetDefinition()
        {
            return Definition;
        }        
    }
}