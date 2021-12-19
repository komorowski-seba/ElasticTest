using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Elastic;

namespace Common.Interfaces
{
    public interface IAirTestElasticService
    {
        Task<List<AirTestElasticModel>> SearchTests(string query);
        void AddDocument(AirTestElasticModel testData);
    }
}