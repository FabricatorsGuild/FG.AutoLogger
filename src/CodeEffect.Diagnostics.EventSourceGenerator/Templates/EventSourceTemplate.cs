﻿namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public partial class EventSourceTemplate
    {
        // ReSharper disable InconsistentNaming

        public const string Variable_EVENTSOURCE_CLASS_NAME = @"@@EVENTSOURCE_CLASS_NAME@@";
        public const string Variable_SOURCE_FILE_NAME = @"@@SOURCE_FILE_NAME@@";
        public const string Variable_EVENTSOURCE_NAME = @"@@EVENTSOURCE_NAME@@";
        public const string Variable_NAMESPACE_DECLARATION = @"@@NAMESPACE_DECLARATION@@";
        public const string Variable_KEYWORDS_DECLARATION = @"@@KEYWORDS_DECLARATION@@";
        public const string Variable_EVENTS_DECLARATION = @"@@EVENTS_DECLARATION@@";
        public const string Variable_EXTENSIONS_DECLARATION = @"@@EXTENSIONS_DECLARATION@@";
        public const string Template_EXTENSIONS_DECLARATION = @"
	internal static class @@EVENTSOURCE_CLASS_NAME@@Helpers
	{
@@EXTENSION_METHODS_DECLARATION@@
	}";
        public const string Variable_EXTENSION_METHODS_DECLARATION = @"@@EXTENSION_METHODS_DECLARATION@@";


        public const string Template_EVENTSOURCE_CLASS_DECLARATION = @"/*******************************************************************************************
*  This class is autogenerated from the class @@SOURCE_FILE_NAME@@
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace @@NAMESPACE_DECLARATION@@
{
	[EventSource(Name = ""@@EVENTSOURCE_NAME@@"")]
	internal sealed partial class @@EVENTSOURCE_CLASS_NAME@@ : EventSource
	{
		public static readonly @@EVENTSOURCE_CLASS_NAME@@ Current = new @@EVENTSOURCE_CLASS_NAME@@();

		static @@EVENTSOURCE_CLASS_NAME@@()
		{
			// A workaround for the problem where ETW activities do not 
			// get tracked until Tasks infrastructure is initialized.
			// This problem will be fixed in .NET Framework 4.6.2.
			Task.Run(() => { });
		}

		// Instance constructor is private to enforce singleton semantics
		private @@EVENTSOURCE_CLASS_NAME@@() : base() { }

		#region Keywords
		// Event keywords can be used to categorize events. 
		// Each keyword is a bit flag. A single event can be 
		// associated with multiple keywords (via EventAttribute.Keywords property).
		// Keywords must be defined as a public class named 'Keywords' 
		// inside EventSource that uses them.
		public static class Keywords
		{
@@KEYWORDS_DECLARATION@@
		}
		#endregion Keywords

		#region Events

@@EVENTS_DECLARATION@@

		#endregion Events
	}

@@EXTENSIONS_DECLARATION@@

}";
        // ReSharper restore InconsistentNaming
    }
}