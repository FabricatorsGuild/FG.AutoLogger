using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Utils
{
    public static class StringMatchExtensions
    {
        private static readonly Regex FindInitialLowercaseRegex = new Regex(@"\b([a-z])", RegexOptions.Compiled);

        private static readonly Regex HumanReadableStringRegex = new Regex(@"((?<=\p{Ll})\p{Lu}|\p{Lu}(?=\p{Ll}))", RegexOptions.Compiled);
        public static bool Matches(this string that, string pattern, StringComparison stringComparison, bool useWildcards = true)
        {
            var regExPattern = pattern;
            if (useWildcards)
            {
                regExPattern = "^" + Regex.Escape(pattern).
                                   Replace("\\*", ".*").
                                   Replace("\\?", ".") + "$";
            }
            var options = RegexOptions.None;
            if (stringComparison == StringComparison.InvariantCultureIgnoreCase || stringComparison == StringComparison.CurrentCultureIgnoreCase ||
                stringComparison == StringComparison.OrdinalIgnoreCase)
            {
                options = RegexOptions.IgnoreCase;
            }
            return Regex.IsMatch(that, regExPattern, options);
        }

        public static string ToMD5(this string that)
        {
            var hash = System.Security.Cryptography.MD5.Create();

            var input = Encoding.UTF8.GetBytes(that);
            var output = hash.ComputeHash(input);

            var result = Encoding.UTF8.GetString(output);
            return result;
        }

        public static string ToBase64(this string that)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(that);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string ToHex(this string that, bool upperCase = false)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(that);
            var result = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static string RemoveFromEnd(this string that, string remove)
        {
            if (that == null) return null;
            if (remove == null) return that;
            var length = that.Length;
            var lengthRemove = remove.Length;
            var lengthResult = length - lengthRemove;
            return that.Substring(0, lengthResult);
        }
        public static string RemoveFromStart(this string that, string remove)
        {
            if (that == null) return null;
            if (remove == null) return that;
            var length = that.Length;
            var lengthRemove = remove.Length;
            var lengthResult = length - lengthRemove;
            return that.Substring(lengthRemove, lengthResult);
        }

        public static string RemoveCommonPrefix(this string that, string compareTo, char componentSeparator)
        {
            var a = that.Split(componentSeparator);
            var b = compareTo.Split(componentSeparator);

            var i = 0;
            var j = 0;

            var result = new StringBuilder();
            while ((i < a.Length) &&(j < b.Length))
            {
                var a_i = a[i];
                var b_j = b[j];

                if (a_i.Equals(b_j))
                {
                    result.Append(a_i);
                    result.Append(componentSeparator);
                }
                i++;
                j++;
            }

            return that.RemoveFromStart(result.ToString());
        }

        public static string GetHumanReadable(this string that)
        {
            return HumanReadableStringRegex.Replace(that, " $1").Trim();
        }

        public static string GetUpperCasedInitial(this string that)
        {
            return $"{that?.Substring(0, 1).ToUpperInvariant()}{that?.Substring(1)}";
        }

        public static string GetLowerCasedInitial(this string that)
        {
            return $"{that?.Substring(0, 1).ToLowerInvariant()}{that?.Substring(1)}";
        }
    }
}