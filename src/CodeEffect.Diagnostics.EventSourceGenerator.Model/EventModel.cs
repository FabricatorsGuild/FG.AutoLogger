using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class EventModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public EventArgumentModel[] Arguments { get; set; }
        public string ReturnType { get; set; }
        public System.Diagnostics.Tracing.EventLevel Level { get; set; }
        public KeywordModel[] Keywords { get; set; }
        public EventOpcode? OpCode { get; set; }
        public string MessageFormatter { get; set; }
        [JsonIgnore]
        public bool HasComplexArguments { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global - Declared in json template
        public EventArgumentModel[] ImplicitArguments { get; set; }
        [JsonIgnore]
        public EventModel CorrelatesTo { get; set; }
        public void InsertImplicitArguments(EventArgumentModel[] implicitArguments)
        {
            this.ImplicitArguments = new EventArgumentModel[implicitArguments.Length];
            var index = 0;
            foreach (var argument in implicitArguments)
            {
                this.ImplicitArguments[index] = new EventArgumentModel(
                    name: argument.Name,
                    type: argument.Type,
                    assignment: argument.Assignment)
                {
                    AssignedCLRType = argument.AssignedCLRType,
                    CLRType = argument.CLRType,
                    IsImplicit = true,
                    IsOverriden = false,
                    TypeTemplate = argument.TypeTemplate,
                    TemplatedParentArgument = argument.TemplatedParentArgument                   
                };
                index++;
            }
        }

        public void OverrideArguments(EventArgumentModel[] overrideArguments)
        {
            var index = 0;
            foreach (var argument in this.Arguments)
            {
                var signature = $"{this.Name}.{argument.Name}";

                var overrideArgument = overrideArguments.FirstOrDefault(a => a.Name.Equals(signature, StringComparison.InvariantCulture));
                if (overrideArgument != null)
                {
                    this.Arguments[index] = new EventArgumentModel()
                    {
                        IsOverriden = true,
                        Name = argument.Name,
                        Type = argument.Type,
                        TypeTemplate = overrideArgument.TypeTemplate,
                        Assignment = overrideArgument.Assignment,
                        CLRType = overrideArgument.CLRType ?? argument.CLRType
                    };
                }
                index++;
            }
        }

        public IEnumerable<EventArgumentModel> GetAllArguments()
        {
            foreach (var implicitArgument in ImplicitArguments ?? new EventArgumentModel[0])
            {
                yield return implicitArgument;
            }
            foreach (var argument in Arguments)
            {
                yield return argument;
            }
        }

        public IEnumerable<EventArgumentModel> GetAllNonImplicitArguments()
        {
            foreach (var argument in Arguments)
            {
                yield return argument;
            }
        }

        public IEnumerable<EventArgumentModel> GetAllArgumentsExpanded(bool directArgumentAssignments = true)
        {
            foreach (var argument in GetAllArguments())
            {
                if (argument.TypeTemplate != null)
                {
                    foreach (var templateArgument in argument.TypeTemplate.Arguments)
                    {
                        var memberName = (argument.IsImplicit && !directArgumentAssignments) ? $"_{argument.Name}" : argument.Name;
                        var assignment = templateArgument.Assignment?.Replace(@"$this", memberName);

                        yield return new EventArgumentModel(
                            name: templateArgument.Name,
                            type: templateArgument.Type,
                            assignment: assignment)
                        {
                            TemplatedParentArgument = argument,
                            CLRType = templateArgument.CLRType,
                            IsImplicit = argument.IsImplicit,
                            AssignedCLRType = templateArgument.Type,
                        };
                    }
                }
                else
                {
                    yield return argument;
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(EventModel)} {this.Name}";
        }
    }
}