namespace CodeEffect.Diagnostics.EventSourceGenerator.Templates
{
    public partial class EventSourceKeywordTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Template_KEYWORD = @"			public const EventKeywords @@KEYWORD_NAME@@ = (EventKeywords)0x@@KEYWORD_INDEX@@L;";
        public const string Template_KEYWORD_NAME = @"@@KEYWORD_NAME@@";
        public const string Template_KEYWORD_INDEX = @"@@KEYWORD_INDEX@@";
        // ReSharper restore InconsistentNaming
    }
}