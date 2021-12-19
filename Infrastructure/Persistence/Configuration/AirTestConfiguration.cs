using Domain.Entities.AirAnalysisContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class AirTestConfiguration : IEntityTypeConfiguration<AirTest>
    {
        public void Configure(EntityTypeBuilder<AirTest> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id).HasDefaultValueSql("NEWID()");
        }
    }
}