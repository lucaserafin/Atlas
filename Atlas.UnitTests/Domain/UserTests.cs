using Atlas.Api.Domain;
using FluentAssertions;
using NetTopologySuite.Geometries;


namespace Atlas.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Should_create_user()
    {
        // Arrange
        var username = "test";
        var point = new Point(1, 1);

        // Act
        var user = new User(username, point);

        // Assert
        user.Should().NotBeNull();
        user.Username.Should().Be(username);
        user.Id.Should().Be(0);
        user.Location.Should().Be(point);
        user.UpdatedAt.Should().BeBefore(DateTime.Now.ToUniversalTime());
        user.UpdatedAt.Should().BeAfter(DateTime.Now.AddMinutes(-1).ToUniversalTime());
    }

    [Fact]
    public void Should_not_create_user()
    {
        // Arrange
        var username = "";
        var point = new Point(1, 1);

        // Act
        var act = () => new User(username, point);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_update_location()
    {
        // Arrange
        var username = "test";
        var point = new Point(1, 1);
        var user = new User(username, point);

        // Act
        var updatedAt = user.UpdatedAt;
        var newPoint = new Point(2, 2);
        user.AssociateLocationData(newPoint);

        // Assert
        user.Should().NotBeNull();
        user.Location.Should().Be(newPoint);
        user.UpdatedAt.Should().BeAfter(updatedAt);
    }
}