namespace CodeEffect.Diagnostics.EventSourceGenerator.Tests
{
    // ReSharper disable InconsistentNaming
    public class With_eventsource_definition
    {
        private string _eventSourceDefinition = @"{
  ""Namespace"": ""CodeEffect.Diagnostics.EventSourceGenerator"",
  ""Name"": ""CE-CodeEffect.Diagnostics.EventSourceGenerator"",
  ""Settings"": {
    ""AutogenerateLoggerInterfaces"": ""true""
  },
  ""Keywords"": [
    ""HostInitialization"",
    ""Domain"",
    ""Infrastructure""
  ],
  ""TypeTemplates"": [
    {
      ""Name"": ""Actor"",
      ""CLRType"": ""Microsoft.ServiceFabric.Actors.Runtime.Actor"",
      ""Arguments"": [
        {
          ""Assignment"": ""$this.GetType().ToString()"",
          ""Name"": ""actorType"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.Id.ToString()"",
          ""Name"": ""actorId"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.CodePackageActivationContext.ApplicationTypeName"",
          ""Name"": ""applicationTypeName"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.CodePackageActivationContext.ApplicationName"",
          ""Name"": ""applicationName"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.ServiceTypeName"",
          ""Name"": ""serviceTypeName"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.ServiceName.ToString()"",
          ""Name"": ""serviceName"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.PartitionId"",
          ""Name"": ""partitionId"",
          ""Type"": ""Guid""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.ReplicaId"",
          ""Name"": ""replicaOrInstanceId"",
          ""Type"": ""long""
        },
        {
          ""Assignment"": ""$this.ActorService.Context.NodeContext.NodeName"",
          ""Name"": ""nodeName"",
          ""Type"": ""string""
        }
      ]
    },
    {
      ""Name"": ""Exception"",
      ""CLRType"": ""System.Exception"",
      ""Arguments"": [
        {
          ""Assignment"": ""$this.Message"",
          ""Name"": ""message"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.Source"",
          ""Name"": ""source"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.GetType().FullName"",
          ""Name"": ""exceptionTypeName"",
          ""Type"": ""string""
        },
        {
          ""Assignment"": ""$this.AsJson()"",
          ""Name"": ""exception"",
          ""Type"": ""string""
        }
      ]
    }
  ]
  ""Events"":
  [
  ]
}";

        public class _and_no_loggers
        {
            public void _should_generate_eventsource_charp_item()
            {
                var projectEventSourceGenerator = new ProjectEventSourceGenerator();
                
            }

        }

    }
    // ReSharper restore InconsistentNaming
}