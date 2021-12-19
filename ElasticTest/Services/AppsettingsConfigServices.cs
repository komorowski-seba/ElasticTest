using Common.Interfaces;
using Common.Models.Settings;
using Microsoft.Extensions.Configuration;

namespace ElasticTest.Services
{
    public class AppsettingsConfigServices : IAppsettingsConfigServices
    {
        public ElasticSettings Elastic { get; init; }
        public DbConnectionStrings DbConnection { get; init; }
        public GiosStationSettings GiosStation { get; init; }

        public AppsettingsConfigServices(IConfiguration configuration)
        {
            Elastic = Bind<ElasticSettings>(configuration, "Elasticsearch");
            DbConnection = Bind<DbConnectionStrings>(configuration, "ConnectionStrings");
            GiosStation = Bind<GiosStationSettings>(configuration, "GiosStation");
        }

        private static T Bind<T>(IConfiguration configuration, string key) where T : new()
        {
            var result = new T();
            configuration.Bind(key, result);
            return result;
        }
    }
}