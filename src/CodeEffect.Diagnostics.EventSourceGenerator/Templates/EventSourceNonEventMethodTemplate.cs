namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public partial class EventSourceNonEventMethodTemplate
    {
        public const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";
        public const string Template_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";
        public const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        public const string Variable_EVENT_NAME = @"@@EVENT_NAME@@";

        public const string Variable_NON_EVENT_METHOD_DECLARATION = @"@@NON_EVENT_METHOD_DECLARATION@@";
        public const string Variable_NON_EVENT_METHOD_ARGUMENTS = @"@@NON_EVENT_METHOD_ARGUMENTS@@";
        public const string Template_NONEVENT_METHOD_ARGUMENT_DELIMITER = @", 
			";
        public const string Variable_NON_EVENT_ASSIGNMENT_ARGUMENTS = @"@@NON_EVENT_ASSIGNMENT_ARGUMENTS@@";
        public const string Template_NON_EVENT_ASSIGNMENT_ARGUMENT_DELIMITER = @", 
					";

        public const string Template_NON_EVENT_METHOD = @"
		[NonEvent]
		public void @@EVENT_NAME@@(
			@@NON_EVENT_METHOD_ARGUMENTS@@)
		{
			if (this.IsEnabled())
			{
				@@EVENT_NAME@@(
					@@NON_EVENT_ASSIGNMENT_ARGUMENTS@@);
			}
		}
";
        // ReSharper restore InconsistentNaming
    }
}