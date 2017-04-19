using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeEffect.Diagnostics.EventSourceGenerator.Model;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Tests
{
    // ReSharper disable InconsistentNaming
    public class With_KeywordJsonConverter
    {
        [Test]
        public void should_serialize_keywords_as_string_array()
        {
            var eventSourceModel = new EventSourceModel();

            eventSourceModel.Keywords = new KeywordModel[]
            {
                new KeywordModel() {Name = "Keyword1", Value = 1},
                new KeywordModel() {Name = "Keyword2", Value = 2},
                new KeywordModel() {Name = "Keyword3", Value = 3},
            };

            var converters = new List<JsonConverter> { new KeywordModelJsonConverter() };
            var jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(eventSourceModel, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = converters
            });

            Console.WriteLine(jsonFile);

            jsonFile.Should().Be(@"{""Keywords"":[""Keyword1"",""Keyword2"",""Keyword3""]}");
        }

        [Test]
        public void should_deserialize_keywords_from_string_array()
        {
            var jsonFile = @"{""Keywords"":[""Keyword1"",""Keyword2"",""Keyword3""]}";

            var converters = new List<JsonConverter> { new KeywordModelJsonConverter() };
            var eventSourceModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSourceModel>(jsonFile, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = converters
            });

            eventSourceModel.Keywords.Should().HaveCount(3);
            for (int i = 1; i < 3; i++)
            {
                eventSourceModel.Keywords[i-1].Name.Should().Be($"Keyword{i}");
                eventSourceModel.Keywords[i-1].Value.Should().Be(null);

            }
        }
    }
    // ReSharper restore InconsistentNaming


    // ReSharper disable InconsistentNaming
    // ReSharper restore InconsistentNaming
}
