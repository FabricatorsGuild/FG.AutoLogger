using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class EventArgumentModel : ICloneable
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
        public bool IsTemplated => TemplatedParentArgument != null;
        [JsonIgnore]
        public bool HasTemplate => TypeTemplate != null;
        [JsonIgnore]
        public bool IsImplicit { get; set; }
        [JsonIgnore]
        public bool IsOverriden { get; set; }
        [JsonIgnore]
        public EventArgumentModel TemplatedParentArgument { get; set; }

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

        public IEnumerable<EventArgumentModel> GetTypeTemplateArguments()
        {
            foreach (var argument in TypeTemplate?.Arguments ?? new EventArgumentModel[0])
            {
                yield return new EventArgumentModel()
                {
                    Name = argument.Name,
                    CLRType = argument.CLRType,
                    AssignedCLRType = argument.AssignedCLRType,
                    Assignment = argument.Assignment,
                    Type = argument.Type,
                    TypeTemplate = argument.TypeTemplate,
                    IsImplicit = this.IsImplicit,
                    TemplatedParentArgument = this,
                    IsOverriden = this.IsOverriden,
                };
            }
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

        public object Clone()
        {
            return new EventArgumentModel()
            {
                Name = this.Name,
                Type = this.Type,
                CLRType = this.CLRType,
                AssignedCLRType = this.AssignedCLRType,
                Assignment = this.Assignment,
                IsImplicit = this.IsImplicit,
                TypeTemplate = this.TypeTemplate,
                TemplatedParentArgument = this.TemplatedParentArgument,
                IsOverriden = this.IsOverriden,
            };
        }
    }
}