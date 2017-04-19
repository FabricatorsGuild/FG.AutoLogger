using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public static class KeywordExtensions
    {
        public static bool Contains(this IEnumerable<KeywordModel> keywords, KeywordModel keyword)
        {
            return keywords.Any(t => t.Name.Equals(keyword.Name, StringComparison.InvariantCultureIgnoreCase));
        }
        public static KeywordModel Find(this IEnumerable<KeywordModel> keywords, string keyword)
        {
            return keywords.FirstOrDefault(t => t.Name.Equals(keyword, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}