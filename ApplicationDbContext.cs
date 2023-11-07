using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entities;

namespace WebAPIAutores
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AutorLibro>().HasKey(al => new { al.AutorId, al.LibroId });
		}

		// Comando para iniciar migraciones desde terminal
		//  dotnet ef migrations add NOMBRE_MIGRACION

		// Actualizar base de datos
		// dotnet ef database update

		public DbSet<Autor> Autores { get; set; }
		public DbSet<Libro> Libros { get; set; }
		public DbSet<Comentario> Comentarios { get; set; }
		public DbSet<AutorLibro> AutoresLibros { get; set; }
		public DbSet<LlaveAPI> LlavesAPI { get; set; }
		public DbSet<Peticion> Peticiones { get; set; }
		public DbSet<RestriccionDominio> RestriccionesDominio { get; set; }
		public DbSet<RestriccionIP> RestricionesIP { get; set; }
	}
}