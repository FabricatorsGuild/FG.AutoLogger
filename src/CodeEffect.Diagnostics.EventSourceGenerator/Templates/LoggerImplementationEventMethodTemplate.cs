namespace CodeEffect.Diagnostics.EventSourceGenerator.Templates
{
    public static class LoggerImplementationEventMethodTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";

        public const string Template_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";
        public const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        public const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";

        public const string Variable_LOGGER_METHOD_NAME = @"@@LOGGER_METHOD_NAME@@";
        public const string Variable_LOGGER_METHOD_ARGUMENTS = @"@@LOGGER_METHOD_ARGUMENTS@@";
        public const string Variable_LOGGER_METHOD_IMPLEMENTATION = @"@@LOGGER_METHOD_IMPLEMENTATION@@";

        public const string Template_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION_DELIMITER = @",
			";


        public const string Template_LOGGER_METHOD = @"
		public void @@LOGGER_METHOD_NAME@@(
			@@LOGGER_METHOD_ARGUMENTS@@)
		{
@@LOGGER_METHOD_IMPLEMENTATION@@    
		}
";
        // ReSharper restore InconsistentNaming
    }
}