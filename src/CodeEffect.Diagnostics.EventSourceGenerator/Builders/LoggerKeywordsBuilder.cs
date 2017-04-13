using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
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
            var loggerKeyword = model.GetKeyword();
            model.SourceFileName = model.Name.Substring(1);
            var allKeywords = new List<KeywordModel>();
            allKeywords.AddRange(eventSource.Keywords);
            var nextKeyword = allKeywords.Any() ? allKeywords.Max(kw => kw.Value ?? 1) * 2 : 1;
            if (eventSource.Keywords.Find(loggerKeyword) == null)
            {
                allKeywords.Add(new KeywordModel() { Name = loggerKeyword, Value = nextKeyword });
            }

            eventSource.Keywords = allKeywords.ToArray();
        }
    }
}