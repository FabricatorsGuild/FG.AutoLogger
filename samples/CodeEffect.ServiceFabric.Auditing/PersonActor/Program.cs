﻿using System;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace PersonActor
{
	internal static class Program
	{
		/// <summary>
		/// This is the entry point of the service host process.
		/// </summary>
		private static void Main()
		{
			try
			{
				// This line registers an Actor Service to host your actor class with the Service Fabric runtime.
				// The contents of your ServiceManifest.xml and ApplicationManifest.xml files
				// are automatically populated when you build this project.
				// For more information, see https://aka.ms/servicefabricactorsplatform

				ActorRuntime.RegisterActorAsync<PersonActor>(
				   (context, actorType) => new PersonActorService(context, actorType)).GetAwaiter().GetResult();

				Thread.Sleep(Timeout.Infinite);
			}
			catch (Exception e)
			{
				// TODO: Fix this logging, add option to build ActorOrActorServiceDescription from FabricRuntime?
				//FabricRuntime.GetNodeContext().
				
				new ActorLogger(null).ActorHostInitializationFailed(e);
				throw;
			}
		}
	}
}
