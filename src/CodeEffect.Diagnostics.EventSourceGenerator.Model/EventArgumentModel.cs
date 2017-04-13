using System;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class EventArgumentModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public string CLRType { get; set; }
        [JsonIgnore]
        public string AssignedCLRType { get; set; }
        public string Assignment { get; set; }
        public TypeTemplateModel TypeTemplate { get; set; }
        [JsonIgnore]
        public bool IsTemplated { get; set; }
        [JsonIgnore]
        public bool IsImplicit { get; set; }
        [JsonIgnore]
        public bool IsOverriden { get; set; }

        public EventArgumentModel()
        {
            
        }
        public EventArgumentModel(string name, string type, string assignment)
        {
            Assignment = assignment;
            Name = name;
            Type = type;
            CLRType = type;
        }


        public override string ToString()
        {
            return $"{nameof(EventArgumentModel)} {this.Name} {this.Type}/{this.CLRType}";
        }


        public Type ParseType()
        {
            return ParseType(this.Type);
        }

        public static Type ParseType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                    return typeof(string);
                case ("int"):
                case ("system.int32"):
                    return typeof(int);
                case ("long"):
                case ("system.int64"):
                    return typeof(long);
                case ("bool"):
                case ("system.boolean"):
                    return typeof(bool);
                case ("datetime"):
                case ("system.dateTime"):
                    return typeof(System.DateTime);
                case ("guid"):
                case ("system.guid"):
                    return typeof(Guid);
                default:
                    return typeof(object);
            }
        }
        public bool IsComplexType()
        {
            var type = ParseType();
            return type == typeof(object);
        }
    }
}