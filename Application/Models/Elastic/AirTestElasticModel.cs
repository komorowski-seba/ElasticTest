using System;

namespace Application.Models.Elastic
{
    public class AirTestElasticModel
    {
        public long Id { get; init; }
        public string CityName { get; init; }
        public DateTime CalcDate { get; init; }
        public DateTime DownloadDate { get; init; }
        public int So2IndexLevel { get; init; }
        public string So2IndexName { get; init; }
        public int No2IndexLevel { get; init; }
        public string No2IndexName { get; init; }
        public int Pm10IndexLevel { get; init; }
        public string Pm10IndexName { get; init; }
        public int Pm25IndexLevel { get; init; }
        public string Pm25IndexName { get; init; }
        public int O3IndexLevel { get; init; }
        public string O3IndexName { get; init; }
    }
}