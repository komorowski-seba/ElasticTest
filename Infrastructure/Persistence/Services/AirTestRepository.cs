using System.Collections.Generic;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities.AirAnalysisContext;

namespace Infrastructure.Persistence.Services
{
    public class AirTestRepository : IAirTestRepository
    {
        private readonly IApplicationDbContext _applicationDb;

        public AirTestRepository(IApplicationDbContext applicationDb)
        {
            _applicationDb = applicationDb;
        }

        public void AddAirTestList(IList<AirTest> airTests)
        {
            _applicationDb.AirTests.AddRange(airTests);
        }
    }
}