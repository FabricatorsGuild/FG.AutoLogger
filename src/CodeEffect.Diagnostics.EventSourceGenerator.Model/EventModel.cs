using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Model
{
    public class EventModel
    {
        private readonly HashSet<string> _implicitArgumentsAdded = new HashSet<string>();

        public int? Id { get; set; }
        public string Name { get; set; }
        public EventArgumentModel[] Arguments { get; set; }
        public System.Diagnostics.Tracing.EventLevel Level { get; set; }
        public KeywordModel[] Keywords { get; set; }
        public string MessageFormatter { get; set; }
        [JsonIgnore]
        public bool HasComplexArguments { get; set; }
        public EventArgumentModel[] ImplicitArguments { get; set; }
        public void InsertImplicitArguments(EventArgumentModel[] implicitArguments)
        {
            var hash = implicitArguments.Aggregate("", (a, i) => $"{a}{i.Name}");
            if (_implicitArgumentsAdded.Contains(hash)) return;

            this.ImplicitArguments = implicitArguments.ToArray();

            _implicitArgumentsAdded.Add(hash);
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

        public IEnumerable<EventArgumentModel> GetAllArgumentsExpanded()
        {
            foreach (var argument in GetAllArguments())
            {
                if (argument.TypeTemplate != null)
                {
                    foreach (var templateArgument in argument.TypeTemplate.Arguments)
                    {
                        yield return new EventArgumentModel(
                            name: templateArgument.Name,
                            type: templateArgument.Type,
                            assignment: templateArgument.Assignment?.Replace(@"$this", argument.Name))
                        {
                            TemplatedParentArgument = argument,
                            CLRType = templateArgument.CLRType,
                            IsImplicit = argument.IsImplicit,
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