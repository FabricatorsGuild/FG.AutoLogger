using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public static class EventSourceModelExtensions
    {
        public static KeywordModel GetKeyword(this IEnumerable<KeywordModel> that, string keywordName)
        {
            return that.FirstOrDefault(k => k.Name.Equals(keywordName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static KeywordModel GetKeyword(this EventSourceModel that, string keywordName)
        {
            return that.Keywords.GetKeyword(keywordName);
        }

        public static KeywordModel AddKeyword(this EventSourceModel that, string keywordName)
        {
            var maxKeywordValue = that.Keywords.Max(k => k.Value) ?? 0;
            var newKeyword = new KeywordModel()
            {
                Name = "Error",
                Value = (maxKeywordValue == 0 ? 1 : maxKeywordValue * 2)
            };
            var keywords = new List<KeywordModel>(that.Keywords) {newKeyword};
            that.Keywords = that.Keywords.Add(newKeyword);

            return newKeyword;
        }

        private static T[] Add<T>(this T[] that, T item)
        {
            var list = new List<T>(that) { item };
            return list.ToArray();
        }
    }
}