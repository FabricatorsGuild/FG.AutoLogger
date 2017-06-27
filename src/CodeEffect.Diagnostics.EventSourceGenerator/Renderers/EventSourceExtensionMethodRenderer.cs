using FG.Diagnostics.AutoLogger.Generator.Templates;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Renderers
{
    public class EventSourceExtensionMethodRenderer : BaseWithLogging, IExtensionsMethodRenderer
    {
        public string Render(Project project, EventSourceModel eventSource, ExtensionsMethodModel model)
        {
            if (model.Type == "AsJson")
            {
                var output = EventSourceExtensionMethodTemplate.Template_EXTENSION_ASJSON_DECLARATION;
                output = output.Replace(EventSourceExtensionMethodTemplate.Template_EXTENSION_CLRTYPE, model.CLRType);

                return output.ToString();
            }
            else if (model.Type == "GetReplicaOrInstanceId")
            {
                var output = EventSourceExtensionMethodTemplate.Template_EXTENSION_GETREPLICAORINSTANCEID_DECLARATION;
                return output.ToString();
            }
            else if (model.Type == "GetContentDigest")
            {
                var output = EventSourceExtensionMethodTemplate.Template_EXTENSION_GETCONTENTDIGEST_DECLARATION;
                return output.ToString();
            }
            else if (model.Type == "GetMD5Hash")
            {
                var output = EventSourceExtensionMethodTemplate.Template_EXTENSION_GETMD5HASH_DECLARATION;
                return output.ToString();
            }
            return null;

        }
    }
}