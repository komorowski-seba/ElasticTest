using System.Reflection;
using Application.Interfaces;
using Domain.Entities.AirAnalysisContext;
using Domain.Entities.ProvinceContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext  : DbContext, IApplicationDbContext
    {
        public DbSet<Province> Provinces { get; init; }
        public DbSet<Commune> Communes { get; init; }
        public DbSet<City> Cities { get; init; }
        public DbSet<Station> Stations { get; init; }
        public DbSet<AirTest> AirTests { get; init; }

        public ApplicationDbContext(DbContextOptions options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}