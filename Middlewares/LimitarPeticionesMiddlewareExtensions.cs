using WebAPIAutores.DTOs;

namespace WebAPIAutores.Middlewares
{
	public static class LimitarPeticionesMiddlewareExtensions
	{
		public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
		{
			return app.UseMiddleware<LimitarPeticionesMiddleware>();
		}
	}

	public class LimitarPeticionesMiddleware
	{
		private readonly RequestDelegate siguiente;
		private readonly IConfiguration configuration;

		public LimitarPeticionesMiddleware(RequestDelegate siguiente, IConfiguration configuration)
		{
			this.siguiente = siguiente;
			this.configuration = configuration;
		}

		public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
		{
			var limitarPeticionConfiguracion = new LimitarPeticionesConfiguracion();
			configuration.GetRequiredSection("limitarPeticiones").Bind(limitarPeticionConfiguracion);

			var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

			if (llaveStringValues.Count == 0)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
				return;
			}
			await siguiente(httpContext);
		}
	}
}
