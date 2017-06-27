using System;
using System.Collections.Generic;
using System.Linq;

namespace FG.Diagnostics.AutoLogger.Model
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
                Name = keywordName,
                Value = (maxKeywordValue == 0 ? 1 : maxKeywordValue * 2)
            };
            that.Keywords = that.Keywords.Add(newKeyword);

            return newKeyword;
        }

        public static EventTaskModel GetEventTask(this IEnumerable<EventTaskModel> that, string keywordName)
        {
            return that.FirstOrDefault(k => k.Name.Equals(keywordName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static EventTaskModel GetEventTask(this EventSourceModel that, string keywordName)
        {
            return that.Tasks.GetEventTask(keywordName);
        }

        public static EventTaskModel AddEventTask(this EventSourceModel that, string eventTaskName)
        {
            var maxEventTaskValue = that.Tasks.Max(k => k.Value) ?? 0;
            var newEventTask = new EventTaskModel()
            {
                Name = eventTaskName,
                Value = (maxEventTaskValue == 0 ? 1 : maxEventTaskValue * 2)
            };
            that.Tasks = that.Tasks.Add(newEventTask);

            return newEventTask;
        }

        private static T[] Add<T>(this T[] that, T item)
        {
            var list = new List<T>(that) { item };
            return list.ToArray();
        }
    }
}