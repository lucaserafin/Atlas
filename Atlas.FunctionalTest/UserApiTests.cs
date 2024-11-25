using Atlas.Api.Application.Commands.Poi;
using Atlas.Api.Application.Commands.User;
using Atlas.Api.Application.Dto;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Atlas.FunctionalTest;

public class UserApiTests : BaseIntegrationTest
{
    public UserApiTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_create_and_retrieve_users()
    {
        // Arrange
        var client = _factory.CreateClient();

        var user = new CreateUserRequest
        (
            Username: "Test User",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/users", user);
        response.EnsureSuccessStatusCode();

        var responseUser = await client.GetFromJsonAsync<UserDto>(response.Headers.Location.ToString());

        // Assert
        responseUser.Should().NotBeNull();
        responseUser.Username.Should().Be(user.Username);
        responseUser.Latitude.Should().Be(user.Coordinate.Latitude);
        responseUser.Longitude.Should().Be(user.Coordinate.Longitude);
    }

    [Fact]
    public async Task Should_create_and_retrieve_multiple_users()
    {
        // Arrange
        var client = _factory.CreateClient();

        var user = new CreateUserRequest
        (
            Username: "Test User2",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );
        var user2 = new CreateUserRequest
        (
           Username: "Test User3",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );
        // Act
        var response = await client.PostAsJsonAsync("/api/users", user);
        response.EnsureSuccessStatusCode();
        response = await client.PostAsJsonAsync("/api/users", user2);
        response.EnsureSuccessStatusCode();

        var responseUser = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users");

        // Assert
        responseUser.Should().NotBeNull();
        responseUser.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public async Task Should_update_user()
    {
        // Arrange
        var client = _factory.CreateClient();

        var responseUser = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users");
        var firstUser = responseUser.First();
        var userUpdated = firstUser with { Username = "updated" };

        // Act
        var response = await client.PutAsJsonAsync("/api/users/" + userUpdated.Guid, userUpdated);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseUserUpdated = await client.GetFromJsonAsync<UserDto>("/api/users/" + userUpdated.Guid);
        responseUserUpdated.Should().NotBeNull();
        responseUserUpdated.Username.Should().Be(userUpdated.Username);
    }

    [Fact]
    public async Task Should_delete_user()
    {
        // Arrange
        var client = _factory.CreateClient();

        var responseUser = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users");
        var firstUser = responseUser.First();

        // Act
        var response = await client.DeleteAsync("/api/users/" + firstUser.Guid);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseUserAfterDelete = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users");
        responseUserAfterDelete.Should().NotContain(u => u.Guid == firstUser.Guid);
    }

    [Fact]
    public async Task Should_return_bad_request_when_creating_user_with_invalid_data()
    {
        // Arrange
        var client = _factory.CreateClient();

        var user = new CreateUserRequest
        (
            Username: "", // Invalid username
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/users", user);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_return_not_found_when_updating_non_existent_user()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userUpdated = new UserDto
        (
            Guid: Guid.NewGuid(), // Non-existent user
            Username: "updated",
            Latitude: 10.0,
            Longitude: 20.0
        );

        // Act
        var response = await client.PutAsJsonAsync("/api/users/" + userUpdated.Guid, userUpdated);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_return_not_found_when_deleting_non_existent_user()
    {
        // Arrange
        var client = _factory.CreateClient();

        var nonExistentUserId = Guid.NewGuid(); // Non-existent user

        // Act
        var response = await client.DeleteAsync("/api/users/" + nonExistentUserId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_update_user_location()
    {
        // Arrange
        var client = _factory.CreateClient();

        var responseUser = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users");
        var firstUser = responseUser.First();
        var newCoordinate = new CoordinateDto(Latitude: 30.0, Longitude: 40.0);

        // Act
        var response = await client.PutAsJsonAsync($"/api/users/{firstUser.Guid}/location", newCoordinate);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseUserUpdated = await client.GetFromJsonAsync<UserDto>($"/api/users/{firstUser.Guid}");
        responseUserUpdated.Should().NotBeNull();
        responseUserUpdated.Latitude.Should().Be(newCoordinate.Latitude);
        responseUserUpdated.Longitude.Should().Be(newCoordinate.Longitude);
    }


    [Fact]
    public async Task Should_get_near_point_of_interest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var responseUser = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users");
        var firstUser = responseUser.First();
        var distance = 5.0; // 5 km
        var pointOfInterest = new CreatePointOfInterestRequest
        (
            Name: "Test POI",
            Description: "Test POI Description",
            Coordinate: new CoordinateDto(Latitude: 10.0, Longitude: 20.0)
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/pointofinterests", pointOfInterest);
        response.EnsureSuccessStatusCode();
        // Act 2
        response = await client.GetAsync($"/api/users/{firstUser.Guid}/PointOfInterest?distance={distance}");

        // Assert
        response.EnsureSuccessStatusCode();
        var pointsOfInterest = await response.Content.ReadFromJsonAsync<IEnumerable<PointOfInterestDto>>();
        pointsOfInterest.Should().NotBeNull();
        pointsOfInterest.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_get_users_in_radius_area()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var responseUsers = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users/userinradius?latitude=1&longitude=1&distance=10000");

        // Assert
        responseUsers.Should().NotBeNullOrEmpty();
    }


    [Fact]
    public async Task Should_get_users_in_region_area()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = new CreateUserRequest
        (
            Username: "TestUserAt0",
            Coordinate: new CoordinateDto(Latitude: 0, Longitude: 0)
        );

        await client.PostAsJsonAsync("/api/users", user);

        // Act
        var responseUsers = await client.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users/usersinregion?Latitude0=0&Longitude0=0&Latitude1=0&Longitude1=1&Latitude2=1&Longitude2=1&Latitude3=1&Longitude3=0&Latitude4=0&Longitude4=0");

        // Assert
        responseUsers.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_get_distance_between_user_and_poi()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = new CreateUserRequest
        (
            Username: "TestUserAt1",
            Coordinate: new CoordinateDto(Latitude: 1, Longitude: 1)
        );

        var userResponse = await client.PostAsJsonAsync("/api/users", user);
        userResponse.EnsureSuccessStatusCode();
        var userDto = await client.GetFromJsonAsync<UserDto>(userResponse.Headers.Location.ToString());

        // Arrange2
        var pointOfInterest = new CreatePointOfInterestRequest
        (
            Name: "TestPOIat0",
            Description: "Test POI Description",
            Coordinate: new CoordinateDto(Latitude: 0, Longitude: 0)
        );
        var responsePoi = await client.PostAsJsonAsync("/api/pointofinterests", pointOfInterest);
        responsePoi.EnsureSuccessStatusCode();
        var poiDto = await client.GetFromJsonAsync<PointOfInterestDto>(responsePoi.Headers.Location.ToString());

        // Act
        var responsedistance = await client.GetFromJsonAsync<double>($"/api/users/{userDto.Guid}/distance/pointofinterest/{poiDto.Guid}");

        // Assert
        responsedistance.Should().BeApproximately(157,1);
    }
}
