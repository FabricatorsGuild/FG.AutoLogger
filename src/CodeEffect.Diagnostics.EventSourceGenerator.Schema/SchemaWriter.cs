using System.Threading.Tasks;
using FG.Diagnostics.AutoLogger.Model;
using NJsonSchema;
using NJsonSchema.Generation;

namespace FG.Diagnostics.AutoLogger.Schema
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
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            var schemaFileName = $"{projectNamespace}.json";
            var schemaPath = System.IO.Path.Combine(currentDirectory, schemaFileName);
            System.IO.File.WriteAllText(schemaPath, schemaJson);

            var projectItem = new ProjectItem(type: ProjectItemType.Unknown, name: schemaFileName)
            {
                Output = schemaJson,
            };
            return projectItem;
        }
    }
}
