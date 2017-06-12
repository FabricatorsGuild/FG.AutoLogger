namespace PersonActor.Diagnostics
{
	public interface IPersonActorLogger
	{
		void PersonActivated(string name, string title);
		void PersonLoaded();
		void TitleSet(string title); 
	}
}