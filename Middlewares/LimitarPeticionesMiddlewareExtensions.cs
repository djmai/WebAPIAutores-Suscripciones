using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

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

			if (llaveStringValues.Count > 1)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsync("Solo una llave debe de estar presente");
				return;
			}

			var llave = llaveStringValues[0];

			var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Llave == llave);

			if (llaveDB == null)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsync("La llave no existe");
				return;
			}

			if (!llaveDB.Activa)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
				return;
			}

			if (llaveDB.TipoLlave == Entities.TipoLlave.Gratuita)
			{
				var hoy = DateTime.Today;
				var mañana = hoy.AddDays(1);
				var cantidadPeticionesRealizadasHoy = await context.Peticiones
					.CountAsync(x => x.LlaveId == llaveDB.Id && x.FechaPeticion >= hoy && x.FechaPeticion <= mañana);

				if (cantidadPeticionesRealizadasHoy >= limitarPeticionConfiguracion.PeticionesPorDiaGratuito)
				{
					httpContext.Response.StatusCode = 429; // Too many requests
					await httpContext.Response.WriteAsync("Ha excedido el límite de peticiones por día. Si desea realizar más peticiones, " +
						"actualice su suscripción a una cuenta profesional");
					return;
				}
			}

			var peticion = new Peticion() { LlaveId = llaveDB.Id, FechaPeticion = DateTime.UtcNow };
			context.Add(peticion);
			await context.SaveChangesAsync();

			await siguiente(httpContext);
		}
	}
}
