namespace CodeEffect.Diagnostics.EventSourceGenerator.MSBuild
{
    public class EventSourceKeyword
    {
        public string Name { get; set; }
        public const string Template_KEYWORD = @"			public const EventKeywords @@KEYWORD_NAME@@ = (EventKeywords)0x@@KEYWORD_INDEX@@L;";
        public const string Template_KEYWORD_NAME = @"@@KEYWORD_NAME@@";
        public const string Template_KEYWORD_INDEX = @"@@KEYWORD_INDEX@@";

        public string Render(int index)
        {
            var output = Template_KEYWORD;
            output = output.Replace(Template_KEYWORD_NAME, this.Name);
            output = output.Replace(Template_KEYWORD_INDEX, index.ToString());

            return output;
        }
    }
}