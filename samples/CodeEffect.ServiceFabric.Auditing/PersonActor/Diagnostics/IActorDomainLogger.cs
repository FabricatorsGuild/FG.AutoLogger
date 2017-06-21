namespace PersonActor.Diagnostics
{
	public interface IActorDomainLogger
	{
		void PersonActivated(string name, string title);
		void PersonLoaded();
		void TitleSet(string title);		
	}
}