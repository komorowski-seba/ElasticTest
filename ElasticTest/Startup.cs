using System;
using Application.Interfaces;
using ElasticTest.Services;
using Hangfire;
using Infrastructure.Elastic;
using Infrastructure.Hangfire;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ElasticTest
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ElasticTest", Version = "v1"});
            });
            
            services.AddScoped<IGiosStationService, GiosStationService>();
            services.AddScoped<IAppsettingsConfigServices>(p => new AppsettingsConfigServices(Configuration));

            services.AddHangfireServices(Configuration);
            services.AddElasticServices(Configuration);
            services.AddPersistenceServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElasticTest v1"));
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UsePersistenceConfiguration();
            app.UseHangfireConfiguration(serviceProvider);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}