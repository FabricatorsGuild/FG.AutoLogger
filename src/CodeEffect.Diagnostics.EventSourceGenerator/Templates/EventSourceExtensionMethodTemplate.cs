namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public partial class EventSourceExtensionMethodTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Template_EXTENSION_CLRTYPE = @"@@EXTENSION_CLRTYPE@@";

        public const string Template_EXTENSION_ASJSON_DECLARATION = @"
            public static string AsJson(this @@EXTENSION_CLRTYPE@@ that)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(that);
            }
";

        public const string Template_EXTENSION_GETREPLICAORINSTANCEID_DECLARATION = @"
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

        public const string Template_EXTENSION_GETCONTENTDIGEST_DECLARATION = @"
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

        public const string Template_EXTENSION_GETMD5HASH_DECLARATION = @"
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
    }
}