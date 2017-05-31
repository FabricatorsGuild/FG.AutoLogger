/*******************************************************************************************
*  This class is autogenerated from the class Diagnostics\sample.eventsource.json
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace ConsoleApplication1.Diagnostics
{
	[EventSource(Name = "CE-CodeEffect.Diagnostics.EventSourceGenerator-sample")]
	internal sealed partial class Sample : EventSource
	{
		public static readonly Sample Current = new Sample();

		static Sample()
		{
			// A workaround for the problem where ETW activities do not 
			// get tracked until Tasks infrastructure is initialized.
			// This problem will be fixed in .NET Framework 4.6.2.
			Task.Run(() => { });
		}

		// Instance constructor is private to enforce singleton semantics
		private Sample() : base() { }

		#region Keywords
		// Event keywords can be used to categorize events. 
		// Each keyword is a bit flag. A single event can be 
		// associated with multiple keywords (via EventAttribute.Keywords property).
		// Keywords must be defined as a public class named 'Keywords' 
		// inside EventSource that uses them.
		public static class Keywords
		{
			public const EventKeywords HostInitialization = (EventKeywords)0x1L;
			public const EventKeywords Domain = (EventKeywords)0x2L;
			public const EventKeywords Infrastructure = (EventKeywords)0x4L;
			public const EventKeywords Dependency = (EventKeywords)0x8L;

		}
		#endregion Keywords

		#region Events



		#endregion Events
	}



}