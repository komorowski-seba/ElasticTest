using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models.Elastic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElasticTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ElasticController : ControllerBase
    {
        private readonly ILogger<ElasticController> _logger;
        private readonly IAppsettingsConfigServices _appsettingsConfigServices;
        private readonly IAirTestElasticService _testElasticService;

        public ElasticController(ILogger<ElasticController> logger, 
            IAppsettingsConfigServices appsettingsConfigServices, 
            IAirTestElasticService testElasticService)
        {
            _logger = logger;
            _appsettingsConfigServices = appsettingsConfigServices;
            _testElasticService = testElasticService;
        }

        [HttpGet]
        public async Task<IEnumerable<AirTestElasticModel>> Get(string query)
        {
            var result = await _testElasticService.SearchTests(query);
            return result;
        }
    }
}