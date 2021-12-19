using Domain.Entities.AirAnalysisContext;
using Domain.Entities.ProvinceContext;
using Microsoft.EntityFrameworkCore;

namespace Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Province> Provinces { get; init; }
        DbSet<Commune> Communes { get; init; }
        DbSet<City> Cities { get; init; }
        DbSet<Station> Stations { get; init; }
        public DbSet<AirTest> AirTests { get; init; }

        int SaveChanges();
    }
}