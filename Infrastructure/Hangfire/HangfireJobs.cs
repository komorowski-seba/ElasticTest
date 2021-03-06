using System;
using Application.Interfaces;
using Hangfire;

namespace Infrastructure.Hangfire
{
    public class HangfireJobs
    {
        public static void StartJobs()
        {
            // BackgroundJob.Enqueue<IGiosStationService>( stationService => stationService.GetNewTest() );
            // BackgroundJob.Enqueue<IGiosStationService>( stationService => stationService.CompleteAllProvinces() );
            BackgroundJob.Schedule<IGiosService>(stationService => stationService.GetNewTest(), TimeSpan.FromHours(1));
        }
    }
}