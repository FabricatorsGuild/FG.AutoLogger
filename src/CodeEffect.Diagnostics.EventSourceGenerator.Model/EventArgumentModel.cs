using System;
using System.Collections.Generic;
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
    }
}