using System;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Extensions
{
    public class ExceptionTypeTemplateExtension : BaseTemplateExtension<Exception>
    {
        private const string Definition = @"{
                  ""Name"": ""Exception"",
                  ""CLRType"": ""System.Exception"",
                  ""Arguments"": [
                    {
                      ""Name"": ""message"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this.Message""
                    },
                    {
                      ""Name"": ""source"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this.Source""
                    },
                    {
                      ""Name"": ""exceptionTypeName"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this.GetType().FullName""
                    },
                    {
                      ""Name"": ""exception"",
                      ""Type"": ""string"",
                      ""Assignment"": ""$this.AsJson()""
                    }
                  ]
                }";

        protected override string GetDefinition()
        {
            return Definition;
        }

        public override string Module => @"Core";
    }
}