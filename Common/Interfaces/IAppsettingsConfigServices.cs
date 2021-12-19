using Common.Models.Settings;

namespace Common.Interfaces
{
    public interface IAppsettingsConfigServices
    {
        public ElasticSettings Elastic { get; init; }
        public DbConnectionStrings DbConnection { get; init; }
        public GiosStationSettings GiosStation { get; init; }
    }
}