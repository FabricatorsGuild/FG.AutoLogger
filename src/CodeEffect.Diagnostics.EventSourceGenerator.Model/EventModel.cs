using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Newtonsoft.Json;

namespace FG.Diagnostics.AutoLogger.Model
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
        public EventTaskModel Task { get; set; }
        public string MessageFormatter { get; set; }
        [JsonIgnore]
        public bool HasComplexArguments { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global - Declared in json template
        public EventArgumentModel[] ImplicitArguments { get; set; }
        [JsonIgnore]
        public EventModel CorrelatesTo { get; set; }

        public bool IsScopedOperation { get; set; }

        [JsonIgnore]
        public string  OperationName { get; set; }

        public void InsertImplicitArguments(EventArgumentModel[] implicitArguments)
        {
            var eventImplicitArguments = new List<EventArgumentModel>();
            foreach (var argument in implicitArguments)
            {
                if (!this.Arguments.Any(a => a.Name.Equals(argument.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    eventImplicitArguments.Add(new EventArgumentModel(
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
                    });
                }
            }
            this.ImplicitArguments = eventImplicitArguments.ToArray();
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


        public IEnumerable<EventArgumentModel> GetAllNonImplicitArgumentsExpanded(bool directArgumentAssignments = true)
        {
            foreach (var argument in Arguments)
            {
                foreach (var expandedArgument in GetExpandedArgument(argument, directArgumentAssignments))
                {
                    yield return expandedArgument;
                }
            }
        }

        public IEnumerable<EventArgumentModel> GetExpandedArgument(EventArgumentModel argument, bool directArgumentAssignments = true)
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

        public IEnumerable<EventArgumentModel> GetAllArgumentsExpanded(bool directArgumentAssignments = true)
        {
            var yieldedArgumentNames = new List<string>();
            var argumentsExpanded = new List<EventArgumentModel>();
            foreach (var argument in GetAllArguments())
            {
                foreach (var expandedArgument in GetExpandedArgument(argument, directArgumentAssignments))
                {
                    argumentsExpanded.Add(expandedArgument);
                }                
            }

            foreach (var argument in argumentsExpanded.ToArray())
            {
                if (!argumentsExpanded.Any(a => (a != argument) && a.Name.Equals(argument.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    yield return argument;
                }
                else
                {
                    if (!argument.IsImplicit)
                    {
                        if (!yieldedArgumentNames.Any(a => a.Equals(argument.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            yieldedArgumentNames.Add(argument.Name);
                            yield return argument;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(EventModel)} {this.Name}";
        }
    }
}