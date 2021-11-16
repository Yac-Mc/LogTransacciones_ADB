using System;
using System.IO;
using System.Reflection;
using Masiv_API.CustomStartup.CustomAuthenticationHandler;
using Masiv_API.CustomStartup.CustomExceptionMiddleware;
using Masiv_API.Services.Logger;
using Masiv_API.Services.Roulette;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;

namespace Masiv_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Services
            DependenciesInjection(services);
            #endregion
            #region General
            services.AddControllers();
            SettingSwagger(services);            
            services.AddMemoryCache();
            services.AddAuthentication("BasicAuthentication").AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Documentación API Code Ruleta");
                });
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void DependenciesInjection(IServiceCollection services)
        {
            services.AddTransient<ILoggerManager, LoggerManager>();
            services.AddTransient<IRuletaService, RuletaService>();
        }

        private void SettingSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Documentación API Code Ruleta",
                    Description = "Documentación Swagger para API Code Ruleta"
                });
                c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Description = "basic authentication for API. Please insert user name",
                    Name = "Authentication ",
                    In = ParameterLocation.Header,
                    Scheme = "basic",
                    Type = SecuritySchemeType.Http,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Basic"
                            }
                        },
                        new string[] { }
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
