using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAPIAutores.Entities;
using WebAPIAutores.Filters;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Services;
using WebAPIAutores.Utilidades;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebAPIAutores
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Limpia la cache de claims
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers(opciones =>
			{
				opciones.Filters.Add(typeof(FiltroDeException));
				opciones.Conventions.Add(new SwaggerAgrupaPorVersion());

			})
				.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
				.AddNewtonsoftJson()
				.AddXmlDataContractSerializerFormatters(); // Agregando formato para enviar/recibir XML

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			//services.AddEndpointsApiExplorer();

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
						ClockSkew = TimeSpan.Zero
					};
				});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Web API Autores",
					Description = "Aplicación para API Autores",
					Version = "v1",
					// TermsOfService = new Uri("http://tempuri.org/terms"),
					Contact = new OpenApiContact
					{
						Name = "Miguel Martínez",
						Email = "mmartinez@mmartinezdev.com",
						Url = new Uri("http://www.mmartinezdev.com")
					},
					// License = new OpenApiLicense
					// {
					//     Name = "MTI",
					//     Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
					// }
				});

				c.SwaggerDoc("v2", new OpenApiInfo
				{
					Title = "Web API Autores",
					Description = "Aplicación para API Autores",
					Version = "v2",
					// TermsOfService = new Uri("http://tempuri.org/terms"),
					Contact = new OpenApiContact
					{
						Name = "Miguel Martínez",
						Email = "mmartinez@mmartinezdev.com",
						Url = new Uri("http://www.mmartinezdev.com")
					},
					// License = new OpenApiLicense
					// {
					//     Name = "Apache 2.0",
					//     Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
					// }
				});

				// c.OperationFilter<AgregarParametroHATEOAS>();
				// c.OperationFilter<AgregarParametroXVersion>(); // Agregar parametro x-version al swagger

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					BearerFormat = "JWT",
					Description = "Autenticacion JWT",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] {}
					}
				});

				// var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				// var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
				// c.IncludeXmlComments(rutaXML);

				// string[] methodsOrder = new string[] { "get", "post", "put", "patch", "delete", "options", "trace" };
				// c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc.HttpMethod!.ToLower())}");
			});

			services.AddAutoMapper(typeof(Startup));

			// Configuracion de Identity

			services.AddIdentity<Usuario, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			// Configuracion de Autorizacion

			services.AddAuthorization(opciones =>
			{
				opciones.AddPolicy("esAdmin", politica => politica.RequireClaim("esAdmin"));
			});

			services.AddDataProtection();

			services.AddTransient<HashService>();

			// Configuracion de CORS
			services.AddCors(opciones =>
			{
				opciones.AddDefaultPolicy(builder =>
				{
					//builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader()
					builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() // Permitir cualquier origen 
						.WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
				});
			});

			services.AddTransient<GeneradorEnlaces>();
			services.AddTransient<HATEOSAutorFilterAttribute>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			services.AddScoped<ServicioLlaves>(); // Agregando ServicioLlaves
			services.AddHostedService<FacturasHostedService>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseLoguearRespuestaHTTP();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API Autores v1");
				c.SwaggerEndpoint("/swagger/v2/swagger.json", "Web API Autores v2");
			});

			app.UseHttpsRedirection();

			app.UseRouting();

			// Acticación de Cors
			app.UseCors();

			// Activación de LimitarPeticiones
			app.UseLimitarPeticiones();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}