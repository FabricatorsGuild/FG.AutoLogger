using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;

namespace PersonActor.Interfaces
{
	/// <summary>
	/// This interface defines the methods exposed by an actor.
	/// Clients use this interface to interact with the actor that implements it.
	/// </summary>
	public interface IPersonActor : IActor
	{
		Task<Person> GetPersonAsync(CancellationToken cancellationToken);
		Task SetTitleAsync(string title, CancellationToken cancellationToken);
	}

	[DataContract]
	public class Person
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Title { get; set; }
	}

	public interface IPersonActorService : IService
	{
		Task<IDictionary<string, Person>> GetPersons(CancellationToken cancellationToken);
	}
}
