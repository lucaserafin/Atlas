using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using Atlas.Api.Infrastructure.Contracts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

namespace Atlas.UnitTests.Integration;

public class PoiEntityTypeConfigurationTests
{
    private readonly AtlasDbContext _context;
    private readonly IPointOfInterestRepository _poiRepository;
    public PoiEntityTypeConfigurationTests()
    {
        var options = CreateNewContextOptions();
        _context = new AtlasDbContext(options);
        _poiRepository = new PointOfInterestRepository(_context);
    }
    private static DbContextOptions<AtlasDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<AtlasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [Fact]
    public async Task Should_Create_Poi_With_Valid_Configuration()
    {
        // Arrange
        var poi = new PointOfInterest
        (
            name: "testpoi",
            description: "test description",
            location: new Point(0, 0) { SRID = 4326 }
        );

        // Act
        await _poiRepository.AddAsync(poi);
        await _poiRepository.UnitOfWork.SaveChangesAsync();
        var savedPoi = await _poiRepository.GetAsync(poi.Guid);
        // Assert
        savedPoi.Should().NotBeNull();
        savedPoi!.Name.Should().Be("testpoi");
    }

    [Fact]
    public async Task Should_Require_All_Required_Fields()
    {
        // Arrange
        var poi = new PointOfInterest
        (
            name: "testpoi",
            description: "test description",
            location: null
        );

        // Act
        await _poiRepository.AddAsync(poi);
        var act = () => _poiRepository.UnitOfWork.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPois()
    {
        // Arrange
        var pois = new List<PointOfInterest>
            {
                new PointOfInterest("testpoi1", "test description 2",new NetTopologySuite.Geometries.Point(10.0, 20.0)),
                new PointOfInterest("testpoi2", "test description 2",new NetTopologySuite.Geometries.Point(30.0, 40.0))
            };
        await _context.PointsOfInterest.AddRangeAsync(pois);
        await _context.SaveChangesAsync();

        // Act
        var result = await _poiRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Remove_ShouldRemovePoi()
    {
        // Arrange
        var poi = new PointOfInterest
        (
            name: "testpoi",
            description: "test description",
            location: new Point(0, 0) { SRID = 4326 }
        );
        await _context.PointsOfInterest.AddAsync(poi);
        await _context.SaveChangesAsync();

        // Act
        _poiRepository.Remove(poi);
        await _context.SaveChangesAsync();

        // Assert
        var removedPoi = await _context.PointsOfInterest.FirstOrDefaultAsync(x => x.Name == "testpoi");
        removedPoi.Should().BeNull();
    }
    [Fact]
    public async Task Update_ShouldUpdatePoi()
    {
        // Arrange
        var poi = new PointOfInterest
        (
            name: "testpoi",
            description: "test description",
            location: new Point(0, 0) { SRID = 4326 }
        );
        await _context.PointsOfInterest.AddAsync(poi);
        await _context.SaveChangesAsync();

        // Act
        poi.UpdateName("updatedpoi");
        _poiRepository.Update(poi);
        await _context.SaveChangesAsync();

        // Assert
        var updatedPoi = await _context.PointsOfInterest.FirstOrDefaultAsync(x => x.Name == "updatedpoi");
        updatedPoi.Should().NotBeNull();
    }

    [Fact]
    public async Task PoiExistAsync_ShouldReturnTrue_WhenPoiExists()
    {
        // Arrange
        var poi = new PointOfInterest
        (
            name: "testpoi",
            description: "test description",
            location: new Point(0, 0) { SRID = 4326 }
        );
        await _context.PointsOfInterest.AddAsync(poi);
        await _context.SaveChangesAsync();

        // Act
        var result = await _poiRepository.PointOfInterestNameExistAsync("testpoi");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PoiExistAsync_ShouldReturnFalse_WhenNameDoesNotExist()
    {
        // Act
        var result = await _poiRepository.PointOfInterestNameExistAsync("nonexistentpoi");

        // Assert
        result.Should().BeFalse();
    }
}