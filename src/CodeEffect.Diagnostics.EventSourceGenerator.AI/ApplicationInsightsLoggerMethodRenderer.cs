using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using Microsoft.ApplicationInsights;

namespace CodeEffect.Diagnostics.EventSourceGenerator.AI
{
    public class ApplicationInsightsLoggerMethosdRenderer : ILoggerImplementationMethodRenderer
    {
        public string Render(Project project, ProjectItem<LoggerModel> loggerProjectItem, EventModel eventModel)
        {
            throw new NotImplementedException();
        }
    }

    public class TelemetryHelper
    {
        private TelemetryClient _client = new TelemetryClient();


        public void LogEvent()
        {
            //_client.TrackEvent("Hello", new );
        }
    }
}
