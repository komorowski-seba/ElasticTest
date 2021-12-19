using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Hangfire
{
    public static class HangfireExtension
    {
        public static IApplicationBuilder UseHangfireConfiguration(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new [] { new AuthorizationFilter() }
            });
            CleanJobs();
            HangfireJobs.StartJobs();
            return app;
        }

        public static IServiceCollection AddHangfireServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHangfire(globalConfiguration => globalConfiguration
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
            services.AddHangfireServer();
            return services;
        }
        
        private static void CleanJobs()
        {
            using var connection = JobStorage.Current.GetConnection();
            foreach (var recurringJob in connection.GetRecurringJobs())
            {
                RecurringJob.RemoveIfExists(recurringJob.Id);
            }
        }
    }
    
    internal class AuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}