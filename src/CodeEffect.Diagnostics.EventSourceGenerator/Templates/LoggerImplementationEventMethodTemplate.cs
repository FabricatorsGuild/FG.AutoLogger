namespace FG.Diagnostics.AutoLogger.Generator.Templates
{
    public static class LoggerImplementationEventMethodTemplate
    {
        // ReSharper disable InconsistentNaming
        public const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";

        public const string Template_ARGUMENT_CLR_TYPE = @"@@ARGUMENT_CLR_TYPE@@";
        public const string Template_ARGUMENT_NAME = @"@@ARGUMENT_NAME@@";

        public const string Template_METHOD_ARGUMENT_DECLARATION = @"@@ARGUMENT_CLR_TYPE@@ @@ARGUMENT_NAME@@";

        public const string Variable_LOGGER_METHOD_NAME = @"@@LOGGER_METHOD_NAME@@";
        public const string Variable_LOGGER_METHOD_RETURNTYPE = @"@@LOGGER_METHOD_RETURNTYPE@@";
        public const string Variable_LOGGER_METHOD_ARGUMENTS = @"@@LOGGER_METHOD_ARGUMENTS@@";
        public const string Variable_LOGGER_METHOD_IMPLEMENTATION = @"@@LOGGER_METHOD_IMPLEMENTATION@@";

        public const string Template_LOGGER_IMPLICIT_ARGUMENTS_METHOD_DECLARATION_DELIMITER = @",
			";


        public const string Template_LOGGER_METHOD = @"
		public @@LOGGER_METHOD_RETURNTYPE@@ @@LOGGER_METHOD_NAME@@(
			@@LOGGER_METHOD_ARGUMENTS@@)
		{
@@LOGGER_METHOD_IMPLEMENTATION@@    
		}
";

        public const string Variable_SCOPED_LOGGER_METHODS = @"@@SCOPED_LOGGER_METHODS@@";
        public const string Variable_LOGGER_METHOD_SCOPED_START_IMPLEMENTATION = @"@@LOGGER_METHOD_SCOPED_START_IMPLEMENTATION@@";
        public const string Variable_LOGGER_METHOD_SCOPED_STOP_IMPLEMENTATION = @"@@LOGGER_METHOD_SCOPED_STOP_IMPLEMENTATION@@";
        public const string Template_SCOPED_LOGGER_METHOD_WRAPPER = @"
                ScopeWrapperWithAction.Wrap(() =>
		        {
@@LOGGER_METHOD_SCOPED_START_IMPLEMENTATION@@    
		            return new ScopeWrapperWithAction(() =>
		            {
@@LOGGER_METHOD_SCOPED_STOP_IMPLEMENTATION@@    
		            });
		        }),

";

        public const string Template_SCOPED_LOGGER_METHOD = @"

        public @@LOGGER_METHOD_RETURNTYPE@@ @@LOGGER_METHOD_NAME@@(
			@@LOGGER_METHOD_ARGUMENTS@@)
		{
		    return new ScopeWrapper(new IDisposable[]
		    {
@@SCOPED_LOGGER_METHODS@@
		    });
		}
";


        // ReSharper restore InconsistentNaming
    }
}