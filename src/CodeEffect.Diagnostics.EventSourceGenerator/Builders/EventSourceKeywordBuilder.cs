using System.Collections.Generic;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Generator.Builders
{
    public class EventSourceKeywordBuilder : BaseWithLogging, IEventSourceBuilder
    {
        public void Build(Project project, ProjectItem<EventSourceModel> model)
        {
            if (model == null) return;
            var eventSource = model.Content;
            if (eventSource == null)
            {
                LogError($"{model.Name} should have a content of type {typeof(EventSourceModel).Name} set but found {model.Content?.GetType().Name ?? "null"}");
                return;
            }

            var allKeywords = new List<KeywordModel>();
            var nextKeyword = 1;
            foreach (var keyword in eventSource.Keywords ?? new KeywordModel[0])
            {
                keyword.Value = nextKeyword;
                allKeywords.Add(keyword);
                nextKeyword *= 2;
            }
            eventSource.Keywords = allKeywords.ToArray();
        }
    }
}