using Atlas.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly AtlasDbContext DbContext;
    protected readonly IntegrationTestWebAppFactory _factory;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<AtlasDbContext>();

        DbContext.Database.Migrate();
    }
}


public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _msSqlContainer = new PostgreSqlBuilder().WithImage("postgis/postgis:17-3.5").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AtlasDbContext>));

            services.AddDbContextFactory<AtlasDbContext>((IServiceProvider sp, DbContextOptionsBuilder opts) =>
            {
                opts.UseNpgsql(_msSqlContainer.GetConnectionString(),
                    (options) =>
                    {
                        options.EnableRetryOnFailure();
                        options.UseNetTopologySuite();
                    });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}