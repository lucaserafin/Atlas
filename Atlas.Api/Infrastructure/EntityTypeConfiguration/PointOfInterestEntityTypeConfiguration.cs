using Atlas.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Api.Infrastructure.EntityTypeConfiguration;

public class PointOfInterestEntityTypeConfiguration : IEntityTypeConfiguration<PointOfInterest>
{
    public void Configure(EntityTypeBuilder<PointOfInterest> builder)
    {
        builder.ToTable(nameof(User));

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.Location).IsRequired();
        builder.HasIndex(x => x.Location).HasMethod("GIST");
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
