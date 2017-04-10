namespace CodeEffect.Diagnostics.EventSourceGenerator.MSBuild
{
    public class EventSourceEventCustomArgument : EventSourceEventArgument
    {
        public EventSourceEventCustomArgument() { }
        public EventSourceEventCustomArgument(string name, string type, string assignment)
        {
            Assignment = assignment;
            Name = name;
            Type = type;
            CLRType = type;
        }

        public string AssignedCLRType { get; set; }

        public string Assignment { get; set; }

        public override void SetCLRType(EventSourcePrototype eventSource)
        {
            if (this.CLRType != null)
            {
                AssignedCLRType = this.CLRType;
            }

            base.SetCLRType(eventSource);
        }
    }
}