using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Model
{
    public class EventTaskModelJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var eventTask = value as EventTaskModel[];
            if (eventTask == null) return;

            serializer.Serialize(writer, eventTask.Select(k => k.Name));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var list = new List<EventTaskModel>();
            var value = "";
            while ((value = reader.ReadAsString()) != null)
            {
                list.Add(new EventTaskModel() { Name = value });
            }

            return list.ToArray();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EventTaskModel[]);
        }
    }
}