using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public static class EventTaskExtensions
    {
        public static bool Contains(this IEnumerable<EventTaskModel> eventTasks, EventTaskModel eventTask)
        {
            return eventTasks.Any(t => t.Name.Equals(eventTask.Name, StringComparison.InvariantCultureIgnoreCase));
        }
        public static EventTaskModel Find(this IEnumerable<EventTaskModel> eventTasks, string eventTask)
        {
            return eventTasks.FirstOrDefault(t => t.Name.Equals(eventTask, StringComparison.InvariantCultureIgnoreCase));
        }

        public static EventTaskModel[] AddEventTask(this IEnumerable<EventTaskModel> eventTasks, EventTaskModel eventTask)
        {
            return eventTasks.Add(eventTask);
        }

        private static T[] Add<T>(this IEnumerable<T> that, T item)
        {
            var list = new List<T>(that) { item };
            return list.ToArray();
        }
    }
}