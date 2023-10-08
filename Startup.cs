using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebAPIAutores.Filters;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Services;

namespace WebAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeException));
            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //services.AddEndpointsApiExplorer();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddTransient<IService, ServiceA>();

            services.AddTransient<ServiceTransient>();
            services.AddScoped<ServiceScoped>();
            services.AddSingleton<ServiceSingleton>();
            services.AddTransient<MiFiltroDeAccion>();
            services.AddHostedService<EscribirEnArchivo>();

            services.AddResponseCaching();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // options.TokenValidationParameters = new TokenValidationParameters
                    // {
                    //     ValidateIssuer = false,
                    //     ValidateAudience = false,
                    //     ValidateLifetime = true,
                    //     ValidateIssuerSigningKey = true,
                    //     IssuerSigningKey = new SymmetricSecurityKey(
                    //         Encoding.UTF8.GetBytes(Configuration["keyjwt"]!)),
                    //     ClockSkew = TimeSpan.Zero
                    // };
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
                    //     Name = "Apache 2.0",
                    //     Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                    // }
                });

                string[] methodsOrder = new string[] { "get", "post", "put", "patch", "delete", "options", "trace" };
                c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc.HttpMethod!.ToLower())}");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // app.Run(async contexto => {
            //     await contexto.Response.WriteAsync("Estoy interceptando la petición");
            // });
            // app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            app.UseLoguearRespuestaHTTP();

            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la petición");
                });
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API Autores v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}