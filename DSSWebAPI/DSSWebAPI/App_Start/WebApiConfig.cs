using System.Web.Http;
using System.Net.Http.Headers;

namespace DSSWebAPI {
	public static class WebApiConfig {
		public static void Register(HttpConfiguration config) {
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{param}/{selection}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new
			MediaTypeHeaderValue("text/html"));
		}
	}
}
