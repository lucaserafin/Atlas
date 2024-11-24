using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Api.Infrastructure;

public class AtlasDbContext(DbContextOptions<AtlasDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtlasDbContext).Assembly);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PointOfInterest> PointsOfInterest { get; set; }
}

