using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.Elastic;

namespace Application.Interfaces
{
    public interface IAirTestElasticService
    {
        Task<List<AirTestElasticModel>> SearchTests(string query);
        void AddDocument(AirTestElasticModel testData);
    }
}