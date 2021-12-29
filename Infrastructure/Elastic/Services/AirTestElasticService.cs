using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models.Elastic;
using Microsoft.Extensions.Logging;
using Nest;

namespace Infrastructure.Elastic.Services
{
    public class AirTestElasticService : IAirTestElasticService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<AirTestElasticService> _logger;
        private readonly IAppsettingsConfigServices _appsettingsConfig;

        public AirTestElasticService(IElasticClient elasticClient, 
            ILogger<AirTestElasticService> logger, 
            IAppsettingsConfigServices appsettingsConfig)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _appsettingsConfig = appsettingsConfig;
        }

        public async Task<List<AirTestElasticModel>> SearchTests(string query)
        {
            var result = _elasticClient
                .Search<AirTestElasticModel>(n => n
                    .Query(m => m
                        .Match(c => c
                            .Field(f => f.CityName)
                            .Query(query))));

            if (!result.IsValid)
            {
                _logger.LogWarning("[{nameof}] result error: {result}", nameof(SearchTests), result.DebugInformation);
                return new List<AirTestElasticModel>();
            }

            return result.Documents.ToList();
        }

        public void AddDocument(AirTestElasticModel testData)
        {
            var result = _elasticClient.DocumentExists<AirTestElasticModel>(testData.Id, 
                d => d.Index(_appsettingsConfig.Elastic.Index));

            if (result.Exists)
                _elasticClient.UpdateAsync<AirTestElasticModel>(testData.Id, 
                    d => d.Index(_appsettingsConfig.Elastic.Index).Doc(testData));
            else
                _elasticClient.Index(new IndexRequest<AirTestElasticModel>(testData, IndexName.From<AirTestElasticModel>(), testData.Id));
        }
    }
}