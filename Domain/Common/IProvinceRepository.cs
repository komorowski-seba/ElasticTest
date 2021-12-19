using System.Collections.Generic;
using Domain.Entities.ProvinceContext;

namespace Domain.Common
{
    public interface IProvinceRepository : IRepository<Province>
    {
        void AddProvincesList(IList<Province> provinces);
    }
}