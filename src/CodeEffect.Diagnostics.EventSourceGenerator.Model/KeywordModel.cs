using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class KeywordModel
    {
        public string Name { get; set; }
        public int? Value { get; set; }
    }

    public class KeywordModelJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var keywords = value as KeywordModel[];
            if( keywords == null) return;
            
            serializer.Serialize(writer, keywords.Select(k => k.Name));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var list = new List<KeywordModel>();
            var value = "";
            while ((value = reader.ReadAsString()) != null)
            {
                list.Add(new KeywordModel() { Name = value});
            }

            return list.ToArray();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(KeywordModel[]);
        }
    }
}