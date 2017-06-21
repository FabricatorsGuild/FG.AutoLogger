using System;

namespace PersonActor.Diagnostics
{
	public interface IActorLogger
	{
		void StartActorActive(bool firstActivation);
		void StopActorActive();
		IDisposable ReadState(string stateName);
		IDisposable WriteState(string stateName);
		void ActorHostInitializationFailed(Exception ex);
	}
}