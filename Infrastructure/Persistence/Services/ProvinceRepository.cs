using System.Collections.Generic;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities.ProvinceContext;

namespace Infrastructure.Persistence.Services
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly IApplicationDbContext _applicationDb;

        public ProvinceRepository(IApplicationDbContext applicationDb)
        {
            _applicationDb = applicationDb;
        }

        public void AddProvincesList(IList<Province> provinces)
        {
            _applicationDb.Provinces.AddRange(provinces);
        }
    }
}