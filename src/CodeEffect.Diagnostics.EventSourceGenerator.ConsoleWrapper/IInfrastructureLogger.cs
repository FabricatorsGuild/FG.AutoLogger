namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public interface IInfrastructureLogger
    {
        void StartingHost(string hostName);
        void StoppingHost(string hostName);
    }
}