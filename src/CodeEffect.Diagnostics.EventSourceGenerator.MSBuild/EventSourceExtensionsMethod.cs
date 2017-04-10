using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.MSBuild
{
    public class EventSourceExtensionsMethod
    {
        // ReSharper disable InconsistentNaming
        private const string Template_EXTENSION_CLRTYPE = @"@@EXTENSION_CLRTYPE@@";

        private const string Template_EXTENSION_ASJSON_DECLARATION = @"
            public static string AsJson(this @@EXTENSION_CLRTYPE@@ that)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(that);
            }
";

        private const string Template_EXTENSION_GETREPLICAORINSTANCEID_DECLARATION = @"
            public static long GetReplicaOrInstanceId(this System.Fabric.ServiceContext context)
            {
                var stateless = context as System.Fabric.StatelessServiceContext;
                if (stateless != null)
                {
                    return stateless.InstanceId;
                }

                var stateful = context as System.Fabric.StatefulServiceContext;
                if (stateful != null)
                {
                    return stateful.ReplicaId;
                }

                throw new NotSupportedException(""Context type not supported."");
            }
";

        private const string Template_EXTENSION_GETCONTENTDIGEST_DECLARATION = @"
            public static long GetContentDigest(this string content)
            {
                var contentDigest = """";
                try
                {
    				var hash = content.GetMD5Hash();
                    var length = content?.Length ?? 0;
                    contentDigest = $""{content?.Substring(0, 30)?.Replace(""\r"", """")?.Replace(""\n"", """")}... ({length}) [{hash}]"";
                }
                catch (Exception ex)
                {
                    contentDigest = $""Failed to generate digest {ex.Message}"";
                }
                return contentDigest;
            }
";

        private const string Template_EXTENSION_GETMD5HASH_DECLARATION = @"
		    public static string GetMD5Hash(this string input)
		    {
			    var md5Hasher = MD5.Create();
			    var data = md5Hasher?.ComputeHash(Encoding.UTF8.GetBytes(input));
			    var hexStringBuilder = new StringBuilder();
			    for (var i = 0; i < (data?.Length); i++)
			    {
				    hexStringBuilder.Append(data[i].ToString(""x2""));
			    }
			    return hexStringBuilder.ToString();
		    }
";
        // ReSharper restore InconsistentNaming

        public string CLRType { get; set; }
        public string Type { get; set; }

        public string Render()
        {
            if (this.Type == "AsJson")
            {
                var output = Template_EXTENSION_ASJSON_DECLARATION;
                output = output.Replace(Template_EXTENSION_CLRTYPE, this.CLRType);

                return output.ToString();
            }
            else if (this.Type == "GetReplicaOrInstanceId")
            {
                var output = Template_EXTENSION_GETREPLICAORINSTANCEID_DECLARATION;
                return output.ToString();
            }

            return null;
        }

        public override string ToString()
        {
            return $"{nameof(EventSourceExtensionsMethod)} {this.Type}";
        }
    }
    
    public static class EventSourceExtensionsMethodExtensions
    {
        public static void AddKnownExtension(this EventSourcePrototype eventSource, string extensionName, string clrType)
        {
            if (!eventSource.Extensions.Any(ext => ext.CLRType == clrType && ext.Type == extensionName))
            {
                eventSource.Extensions.Add(new EventSourceExtensionsMethod() { CLRType = clrType, Type = extensionName });
            }
        }

        public static void AddKnownExtensions(this EventSourcePrototype eventSource, EventSourceEventCustomArgument customArgument, string templateCLRType)
        {
            if (customArgument.Assignment.Contains("$this.AsJson()"))
            {
                eventSource.AddKnownExtension("AsJson", templateCLRType);
                return;
            }

            if (customArgument.Assignment.Contains("$this.GetReplicaOrInstanceId()"))
            {
                eventSource.AddKnownExtension("GetReplicaOrInstanceId", templateCLRType);
                return;
            }

            if (customArgument.Assignment.Contains("$this.GetContentDigest("))
            {
                eventSource.AddKnownExtension("GetContentDigest", templateCLRType);
                eventSource.AddKnownExtension("GetMD5Hash", templateCLRType);
                return;
            }
            if (customArgument.Assignment.Contains("$this.GetMD5Hash("))
            {
                eventSource.AddKnownExtension("GetMD5Hash", templateCLRType);
                return;
            }
        }
    }
}