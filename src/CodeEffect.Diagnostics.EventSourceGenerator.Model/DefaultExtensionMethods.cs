using System;
using System.Security.Cryptography;
using System.Text;

namespace FG.Diagnostics.AutoLogger.Model
{
    public static class DefaultExtensionMethods
    {
        public static string AsJson(this object that)
        {
            return that == null ? "{}" : Newtonsoft.Json.JsonConvert.SerializeObject(that);
        }
        public static string GetContentDigest(this string content)
        {
            var contentDigest = "";
            try
            {
                var hash = content.GetMD5Hash();
                var length = content?.Length ?? 0;
                contentDigest = $"{ content?.Substring(0, 30)?.Replace("\r", "")?.Replace("\n", "")}... ({ length}) [{hash}]";
            }
            catch (Exception ex)
            {
                contentDigest = $"Failed to generate digest { ex.Message }";
            }
            return contentDigest;
        }
        public static string GetMD5Hash(this string input)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher?.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hexStringBuilder = new StringBuilder();
            for (var i = 0; i < (data?.Length); i++)
            {
                hexStringBuilder.Append(data[i].ToString("x2"));
            }
            return hexStringBuilder.ToString();
        }

    }
}