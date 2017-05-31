namespace PersonActor.Diagnostics
{
	public interface IServiceFabricActorsLogger
	{
		void ActorHostInitializationFailed(string exception);
	}
}