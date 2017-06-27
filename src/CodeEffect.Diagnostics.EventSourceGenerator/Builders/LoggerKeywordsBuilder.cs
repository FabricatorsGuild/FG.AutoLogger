using System.Collections.Generic;
using System.Linq;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class LoggerKeywordsBuilder : BaseWithLogging, ILoggerBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> eventSourceProjectItem, LoggerModel model)
        {
            if (model == null) return;
            var eventSource = eventSourceProjectItem.Content;
            if (eventSource == null)
            {
                LogError($"{eventSourceProjectItem.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {eventSourceProjectItem.Content?.GetType().Name ?? "null"}");
                return;
            }
            var loggerKeyword = model.Keyword;
            model.SourceFileName = model.Name.Substring(1);
            var allKeywords = new List<KeywordModel>();
            allKeywords.AddRange(eventSource.Keywords);
            var nextKeyword = allKeywords.Any() ? allKeywords.Max(kw => kw.Value ?? 1) * 2 : 1;
            var keyword = new KeywordModel() {Name = loggerKeyword, Value = nextKeyword};
            if (eventSource.Keywords.Find(loggerKeyword) == null)
            {
                allKeywords.Add(keyword);
            }

            eventSource.Keywords = allKeywords.ToArray();

            foreach (var loggerEvent in model.Events ?? new EventModel[0])
            {
                loggerEvent.Keywords = loggerEvent.Keywords.Add(keyword);
            }
        }
    }
}