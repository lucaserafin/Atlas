using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Atlas.Api.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;
using Atlas.Api.Application.Queries;
using System.Reflection.Metadata;

namespace Atlas.UnitTests.Integration;

public class UserEntityTypeConfigurationTests
{
    private readonly AtlasDbContext _context;
    private readonly IUserRepository _userRepository;
    public UserEntityTypeConfigurationTests()
    {
        var options = CreateNewContextOptions();
        _context = new AtlasDbContext(options);
        _userRepository = new UserRepository(_context);
    }
    private static DbContextOptions<AtlasDbContext> CreateNewContextOptions()
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
        var user = new User
        (
            username: "testuser",
            location: new Point(0, 0) { SRID = 4326 }
        );

        // Act
        await _userRepository.AddAsync(user);
        await _userRepository.UnitOfWork.SaveChangesAsync();
        var savedUser = await _userRepository.GetAsync(user.Guid);
        // Assert
        savedUser.Should().NotBeNull();
        savedUser!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task Should_Require_All_Required_Fields()
    {
        // Arrange
        var user = new User
        (
            username: "test",
            location: null
        );

        // Act
        await _userRepository.AddAsync(user);
        var act = () => _userRepository.UnitOfWork.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
            {
                new User("testuser1", new NetTopologySuite.Geometries.Point(10.0, 20.0)),
                new User("testuser2", new NetTopologySuite.Geometries.Point(30.0, 40.0))
            };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Remove_ShouldRemoveUser()
    {
        // Arrange
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        _userRepository.Remove(user);
        await _context.SaveChangesAsync();

        // Assert
        var removedUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == "testuser");
        removedUser.Should().BeNull();
    }
    [Fact]
    public async Task Update_ShouldUpdateUser()
    {
        // Arrange
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        user.UpdateUsername("updateduser");
        _userRepository.Update(user);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == "updateduser");
        updatedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task UsernameExistAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.UsernameExistAsync("testuser");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UsernameExistAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Act
        var result = await _userRepository.UsernameExistAsync("nonexistentuser");

        // Assert
        result.Should().BeFalse();
    }
}