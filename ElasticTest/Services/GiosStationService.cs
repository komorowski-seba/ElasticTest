using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Common.Exceptions;
using Common.Interfaces;
using Common.Models.Dto;
using Common.Models.Elastic;
using Common.Models.GiosStationModels;
using Dapper;
using Domain.Common;
using Domain.Entities.AirAnalysisContext;
using Domain.Entities.ProvinceContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Station = Common.Models.GiosStationModels.Station;

namespace ElasticTest.Services
{
    public class GiosStationService : IGiosStationService
    {
        private readonly IProvinceRepository _provinceRepository;
        private readonly IAirTestRepository _testRepository;
        private readonly IApplicationDbContext _applicationDb;
        private readonly IAppsettingsConfigServices _appsettings;
        private readonly ILogger<GiosStationService> _logger;
        private readonly IAirTestElasticService _airTestElasticService;

        public GiosStationService(IProvinceRepository provinceRepository, 
            IApplicationDbContext applicationDb, 
            IAirTestRepository testRepository, 
            IAppsettingsConfigServices appsettings, 
            ILogger<GiosStationService> logger, 
            IAirTestElasticService airTestElasticService)
        {
            _provinceRepository = provinceRepository;
            _applicationDb = applicationDb;
            _testRepository = testRepository;
            _appsettings = appsettings;
            _logger = logger;
            _airTestElasticService = airTestElasticService;
        }

        public void CompleteAllProvinces()
        {
            var result = GetTestStations();
            var provincesNew = new Dictionary<string, Province>();

            foreach (var provinceDto in result)
            {
                var commune = provinceDto.Commune;
                var city = commune.City;
                var station = city.Station;

                if (!provincesNew.TryGetValue(provinceDto.Name, out var province))
                {
                    province = new Province(provinceDto.Name);
                    provincesNew.Add(provinceDto.Name, province);
                }
                province
                    .AddCommune(provinceDto.Name, commune.DistrictName, commune.DistrictName)
                    .AddCity(city.Id, city.Name)
                    .AddStation(station.Id, station.StationName, station.GegrLat, station.GegrLon, station.AddressStreet);
            }
            
            var provincesNewList = provincesNew.Values.ToList();
            if (provincesNewList.Any())
                _provinceRepository.AddProvincesList(provincesNewList);

            _applicationDb.SaveChanges();
        }

        public void GetNewTest()
        {
            var stations = GetAllStationIds();
            var allQualityList = GetAllStationQualityList(stations.ToList());

            var airTests = new List<AirTest>();
            foreach (var quality in allQualityList)
            {
                var curren = new AirTestElasticModel
                {
                    Id = quality.Id, CityName = quality.CityName, CalcDate = quality.CalculateDate.Date,
                    DownloadDate = DateTime.UtcNow,
                    So2IndexLevel = quality.So2IndexLevel?.Value ?? int.MaxValue,
                    So2IndexName = quality.So2IndexLevel?.IndexLevelName ?? "null",
                    No2IndexLevel = quality.No2IndexLevel?.Value ?? int.MaxValue,
                    No2IndexName = quality.No2IndexLevel?.IndexLevelName ?? "null",
                    Pm10IndexLevel = quality.Pm10IndexLevel?.Value ?? int.MaxValue,
                    Pm10IndexName = quality.Pm10IndexLevel?.IndexLevelName ?? "null",
                    Pm25IndexLevel = quality.Pm25IndexLevel?.Value ?? int.MaxValue,
                    Pm25IndexName = quality.Pm25IndexLevel?.IndexLevelName ?? "null",
                    O3IndexLevel = quality.O3IndexLevel?.Value ?? int.MaxValue,
                    O3IndexName = quality.O3IndexLevel?.IndexLevelName ?? "null"
                };
                
                var test = new AirTest(quality.Id, quality.CalculateDate.Date, DateTime.UtcNow,
                    curren.So2IndexLevel, curren.So2IndexName,
                    curren.No2IndexLevel, curren.No2IndexName,
                    curren.Pm10IndexLevel, curren.Pm10IndexName,
                    curren.Pm25IndexLevel, curren.Pm25IndexName,
                    curren.O3IndexLevel, curren.O3IndexName);
                airTests.Add(test);
                
                _airTestElasticService.AddDocument(curren);
            }
            
            if (airTests.Any())
                _testRepository.AddAirTestList(airTests);
            _applicationDb.SaveChanges();
        }

        private IList<ProvinceDto> GetTestStations()
        {
            var client = new RestClient(_appsettings.GiosStation.Stations);
            var request = new RestRequest(Method.GET);
            var response = client.Get(request);
            if (!response.IsSuccessful)
                throw new ResponseException($"[{_appsettings.GiosStation.Stations}] can't respond: {response.ErrorException.Message}");
            
            var stations = JsonConvert.DeserializeObject<IList<Station>>(response.Content);
            var result = MapStationResponseToDto(stations);
            return result;
        }

        private IList<IndexAirQuality> GetAllStationQualityList(IEnumerable<(long, string)> stations)
        {
            var result = new List<IndexAirQuality>();
            foreach (var (stationId, cityName) in stations)
            {
                try
                {
                    var quality = GetStationAirQuality(stationId, cityName);
                    result.Add(quality);
                }
                catch (Exception e)
                {
                    _logger.LogWarning("[{nameof}] can't take air quality: {message}", 
                        nameof(GetStationAirQuality), 
                        e.Message);
                }
            }
            
            return result;
        }
        
        private IndexAirQuality GetStationAirQuality(long stationId, string cityName)
        {
            var client = new RestClient($"{_appsettings.GiosStation.Quality}/{stationId}");
            var request = new RestRequest(Method.GET);
            var response = client.Get(request);
            if (!response.IsSuccessful)
                throw new ResponseException($"[{_appsettings.GiosStation.Quality}] can't respond: {response.ErrorException.Message}");
            
            var result = JsonConvert.DeserializeObject<IndexAirQuality>(response.Content);
            result.CityName = cityName;
            return result;
        }

        private IList<(long, string)> GetAllStationIds()
        {
            using var con = new SqlConnection(_appsettings.DbConnection.DefaultConnection);
            con.Open();
            var stations = con.Query<(long, string)>("Select Stations.Id, [Name] from Stations inner join Cities on Stations.CityId = Cities.Id").ToList();
            return stations;
        }

        private static IList<ProvinceDto> MapStationResponseToDto(IEnumerable<Station> stations)
        {
            var result = new List<ProvinceDto>();
            foreach (var station in stations)
            {
                var city = station.City;
                var commune = city.Commune;
                
                var province = new ProvinceDto
                {
                    Name = commune.ProvinceName,
                    Commune = new CommuneDto
                    {
                        DistrictName = commune.DistrictName,
                        CommuneName = commune.CommuneName,
                        City = new CityDto
                        {
                            Id = city.Id,
                            Name = city.Name,
                            Station = new StationDto
                            {
                                Id = station.Id,
                                StationName = station.StationName,
                                GegrLat = station.GegrLat,
                                GegrLon = station.GegrLon,
                                AddressStreet = station.AddressStreet
                            }
                        }
                    }
                };
                result.Add(province);
            }
            return result;
        }
    }
}