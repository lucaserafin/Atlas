using Atlas.Api.Application.Commands.Poi;
using Atlas.Api.Application.Dto;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Atlas.FunctionalTest;

public class PointOfInterestsApiTests : BaseIntegrationTest
{
    public PointOfInterestsApiTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_create_and_retrieve_point_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var pointOfInterest = new CreatePointOfInterestRequest
        (
            Name: "Test POI",
            Description: "Test POI Description",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/pointofinterests", pointOfInterest);
        response.EnsureSuccessStatusCode();

        var responsePoi = await client.GetFromJsonAsync<PointOfInterestDto>(response.Headers.Location.ToString());

        // Assert
        responsePoi.Should().NotBeNull();
        responsePoi.Name.Should().Be(pointOfInterest.Name);
        responsePoi.Latitude.Should().Be(pointOfInterest.Coordinate.Latitude);
        responsePoi.Longitude.Should().Be(pointOfInterest.Coordinate.Longitude);
    }

    [Fact]
    public async Task Should_create_and_retrieve_multiple_points_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var pointOfInterest = new CreatePointOfInterestRequest
        (
            Name: "Test POI2",
            Description: "Test POI Description2",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/pointofinterests", pointOfInterest);
        response.EnsureSuccessStatusCode();

        var responsePois = await client.GetFromJsonAsync<IEnumerable<PointOfInterestDto>>("/api/pointofinterests");

        // Assert
        responsePois.Should().NotBeNull();
        responsePois.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public async Task Should_update_point_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var responsePois = await client.GetFromJsonAsync<IEnumerable<PointOfInterestDto>>("/api/pointofinterests");
        var firstPoi = responsePois.First();
        var poiUpdated = firstPoi with { Name = "updated" };

        // Act
        var response = await client.PutAsJsonAsync("/api/pointofinterests/" + poiUpdated.Guid, poiUpdated);

        // Assert
        response.EnsureSuccessStatusCode();
        var responsePoiUpdated = await client.GetFromJsonAsync<PointOfInterestDto>("/api/pointofinterests/" + poiUpdated.Guid);
        responsePoiUpdated.Should().NotBeNull();
        responsePoiUpdated.Name.Should().Be(poiUpdated.Name);
    }

    [Fact]
    public async Task Should_delete_point_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var responsePois = await client.GetFromJsonAsync<IEnumerable<PointOfInterestDto>>("/api/pointofinterests");
        var firstPoi = responsePois.First();

        // Act
        var response = await client.DeleteAsync("/api/pointofinterests/" + firstPoi.Guid);

        // Assert
        response.EnsureSuccessStatusCode();
        var responsePoisAfterDelete = await client.GetFromJsonAsync<IEnumerable<PointOfInterestDto>>("/api/pointofinterests");
        responsePoisAfterDelete.Should().NotContain(p => p.Guid == firstPoi.Guid);
    }

    [Fact]
    public async Task Should_return_bad_request_when_creating_point_of_interest_with_invalid_data()
    {
        // Arrange
        var client = _factory.CreateClient();

        var pointOfInterest = new CreatePointOfInterestRequest
        (
            Name: "", // Invalid name
            Description: "Test POI Description",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/pointofinterests", pointOfInterest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_return_not_found_when_updating_non_existent_point_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var poiUpdated = new PointOfInterestDto
        (
            Guid: Guid.NewGuid(), // Non-existent POI
            Name: "updated",
            Description: "Test POI Description",
            Latitude: 10.0,
            Longitude: 20.0
        );

        // Act
        var response = await client.PutAsJsonAsync("/api/pointofinterests/" + poiUpdated.Guid, poiUpdated);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_return_not_found_when_deleting_non_existent_point_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var nonExistentPoiId = Guid.NewGuid(); // Non-existent POI

        // Act
        var response = await client.DeleteAsync("/api/pointofinterests/" + nonExistentPoiId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
