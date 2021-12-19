using System.Collections.Generic;
using Domain.Entities.AirAnalysisContext;

namespace Domain.Common
{
    public interface IAirTestRepository : IRepository<AirTest>
    {
        void AddAirTestList(IList<AirTest> airAnalyses);
    }
}