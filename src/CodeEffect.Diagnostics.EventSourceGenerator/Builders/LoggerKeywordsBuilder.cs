using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Builders
{
    public class LoggerKeywordsBuilder : ILoggerBuilder
    {
        public void Build(Project project, EventSourceModel eventSource, LoggerModel model)
        {
            var loggerKeyword = model.GetKeyword();
            model.SourceFileName = model.Name.Substring(1);
            var allKeywords = new List<KeywordModel>();
            allKeywords.AddRange(eventSource.Keywords);
            var nextKeyword = allKeywords.Max(kw => kw.Value ?? 1) * 2;
            if (eventSource.Keywords.Find(loggerKeyword) == null)
            {
                allKeywords.Add(new KeywordModel() { Name = loggerKeyword, Value = nextKeyword });
            }

            eventSource.Keywords = allKeywords.ToArray();
        }
    }
}