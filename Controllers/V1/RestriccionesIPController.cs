using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;
using WebAPIAutores.Migrations;

namespace WebAPIAutores.Controllers.V1
{
	[ApiController]
	[Route("api/v1/restriccionesip")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class RestriccionesIPController : CustomBaseController
	{
		private readonly ApplicationDbContext context;

		public RestriccionesIPController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpPost]
		public async Task<ActionResult> Post(CrearRestriccionesIPDTO crearRestriccion)
		{
			var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == crearRestriccion.LlaveId);
			if (llaveDB == null)
			{
				return NotFound();
			}

			var usuarioId = ObtenerUsuarioId();

			if (llaveDB.UsuarioId != usuarioId)
			{
				return Forbid();
			}

			var restriccionIP = new RestriccionIP
			{
				LlaveId = crearRestriccion.LlaveId,
				IP = crearRestriccion.IP
			};

			context.Add(restriccionIP);
			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, ActualizarRestriccionIPDTO actualizarRestriccion)
		{
			var restriccionDB = await context.RestricionesIP.Include(x => x.Llave)
				.FirstOrDefaultAsync(x => x.Id == id);
			if (restriccionDB == null)
			{
				return NotFound();
			}

			var usuarioId = ObtenerUsuarioId();

			if (restriccionDB.Llave.UsuarioId != usuarioId)
			{
				return Forbid();
			}
			restriccionDB.IP = actualizarRestriccion.IP;
			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			var restriccionDB = await context.RestricionesIP.Include(x => x.Llave)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (restriccionDB == null)
			{
				return NotFound();
			}

			var usuarioId = ObtenerUsuarioId();

			if (restriccionDB.Llave.UsuarioId != usuarioId)
			{
				return Forbid();
			}

			context.Remove(restriccionDB);
			await context.SaveChangesAsync();
			return NoContent();
		}

	}
}
