using Microsoft.EntityFrameworkCore;

namespace Atlas.Api.Infrastructure;

public class AtlasDbContext : DbContext
{
    public AtlasDbContext(DbContextOptions<AtlasDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtlasDbContext).Assembly);
    }
}

