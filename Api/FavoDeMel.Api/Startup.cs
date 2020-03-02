using FavoDeMel.Api.Dtos.Mappers;
using FavoDeMel.IoC;
using FavoDeMel.Repository.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace FavoDeMel.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string corsPolicy = "CorsPolicy";
        public void ConfigureServices(IServiceCollection services)
        {
            ApiMapper.Configure();
            services.AddControllers();
            services.AddDbContext<RepositoryDbContext>(c => c.UseSqlServer(Configuration.GetConnectionString("connectionName")));
            services.AddRepository();
            services.AddMensageria();
            services.AddServices();
            services.AddJwtBearer(Configuration);
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy,
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                      new OpenApiInfo
                      {
                          Title = "Api Restaurante Favo de Mel",
                          Version = "v1",
                          Description = "API REST Restaurante Favo de Mel",
                          Contact = new OpenApiContact
                          {
                              Name = "Jonathan D. C. Santiago",
                              Url = new Uri("https://github.com/jonathandsantiago/Arquitetura")
                          }
                      });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>() }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Restaurante Favo de Mel");
            });

            app.UseCors(corsPolicy);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
