using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Atlas.Api.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

namespace Atlas.UnitTests.Integration;

public class UserEntityTypeConfigurationTests
{
    private DbContextOptions<AtlasDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<AtlasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [Fact]
    public async Task Should_Create_User_With_Valid_Configuration()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new AtlasDbContext(options);
        var repo = new UserRepository(context);

        var user = new User
        (
            username: "testuser",
            location:  new Point(0, 0) { SRID = 4326 }
        );

        // Act
        await repo.Add(user);
        await repo.UnitOfWork.SaveChangesAsync();
        var savedUser = await repo.GetAsync(user.Guid);
        // Assert
        savedUser.Should().NotBeNull();
        savedUser!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task Should_Require_All_Required_Fields()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new AtlasDbContext(options);
        var repo = new UserRepository(context);
        var user = new User
        (
            username: "test",
            location: null
        );

        // Act
        await repo.Add(user);
        var act = () => repo.UnitOfWork.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}