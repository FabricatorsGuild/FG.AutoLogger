using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using NJsonSchema;
using NJsonSchema.Generation;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Schema
{
    public static class SchemaWriter
    {
        public static async Task<ProjectItem> GenerateSchema(bool saveChanges = true)
        {
            var schema = await JsonSchema4.FromTypeAsync<EventSourceModel>(new JsonSchemaGeneratorSettings()
            {

            });
            var schemaJson = schema.ToJson();

            var projectNamespace = typeof(EventSourceModel).Namespace;
            var schemaFileName = $"{projectNamespace}.json";
            System.IO.File.WriteAllText(schemaFileName, schemaJson);

            var projectItem = new ProjectItem(type: ProjectItemType.Unknown, name: schemaFileName)
            {
                Output = schemaJson,
            };
            return projectItem;
        }
    }
}
