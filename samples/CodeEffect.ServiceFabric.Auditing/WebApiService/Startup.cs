using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using TinyIoC;

namespace WebApiService
{
	public static class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public static void ConfigureApp(IAppBuilder appBuilder, TinyIoCContainer container)
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration();

			var defaultSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Converters = new List<JsonConverter>
						{
							new StringEnumConverter{ CamelCaseText = true },
						}
			};

			JsonConvert.DefaultSettings = () => defaultSettings;

			config.Formatters.Clear();
			config.Formatters.Add(new JsonMediaTypeFormatter());
			config.Formatters.JsonFormatter.SerializerSettings = defaultSettings;

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.DependencyResolver = new TinyIoCResolver(container);

			appBuilder.UseWebApi(config);
		}
	}
}
