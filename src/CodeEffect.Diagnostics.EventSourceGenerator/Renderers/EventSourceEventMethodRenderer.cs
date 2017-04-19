using System;
using System.Collections.Generic;
using System.Linq;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using CodeEffect.Diagnostics.EventSourceGenerator.Utils;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Renderers
{
    public class EventSourceEventMethodRenderer : EventSourceEventMethodRenderBase, IEventRenderer
    {
        public string Render(Project project, EventSourceModel eventSource, EventModel model)
        {
            return Render(model);
        }
    }
}