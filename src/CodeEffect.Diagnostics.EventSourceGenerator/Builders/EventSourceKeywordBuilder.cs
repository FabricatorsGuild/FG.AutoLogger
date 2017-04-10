using System.Collections.Generic;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class EventSourceKeywordBuilder : IEventSourceBuilder
    {
        public void Build(Project project, EventSourceModel model)
        {
            var allKeywords = new List<KeywordModel>();
            var nextKeyword = 1;
            foreach (var keyword in model.Keywords)
            {
                keyword.Value = nextKeyword;
                allKeywords.Add(keyword);
                nextKeyword *= 2;
            }
            model.Keywords = allKeywords.ToArray();
        }
    }
}