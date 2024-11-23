using Atlas.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Api.Infrastructure.EntityTypeConfiguration;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Guid).IsRequired();
        builder.HasIndex(x => x.Guid).IsUnique();
        builder.Property(x => x.Username).IsRequired();
        builder.Property(x => x.Location).HasColumnType("geography (point)").IsRequired();
        builder.HasIndex(x => x.Location).HasMethod("GIST");

        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
