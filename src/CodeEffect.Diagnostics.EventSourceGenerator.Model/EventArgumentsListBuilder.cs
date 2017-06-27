using System;

namespace FG.Diagnostics.AutoLogger.Model
{
    public class EventArgumentsListBuilder : ListBuilder
    {
        private readonly Func<EventArgumentModel, string> _renderer;

        public EventArgumentsListBuilder(Func<EventArgumentModel, string> renderer, string delimiter, string initialDelimiter = "")
            : this("", renderer, delimiter, initialDelimiter)
        {
        }
        public EventArgumentsListBuilder(string initialContent, Func<EventArgumentModel, string> renderer, string delimiter, string initialDelimiter = "")
            : base(initialContent, delimiter, initialDelimiter)
        {
            _renderer = renderer;
        }
        public EventArgumentsListBuilder(string initialContent = "", string delimiter = "", string initialDelimiter = "")
            : this(initialContent, (arg) => arg.Name, delimiter, initialDelimiter)
        {
        }
        public void Append(EventArgumentModel argument)
        {
            var renderedArgument = _renderer(argument);
            Builder.Append($"{CurrentDelimiter}{renderedArgument}");
            CurrentDelimiter = Delimiter;
        }

    }
}