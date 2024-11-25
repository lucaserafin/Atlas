using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Infrastructure.Contracts;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Atlas.Api.Domain;

namespace Atlas.UnitTests.Application;

public class GetUsersInRegionRequestHandlerTests
{
    private readonly ILogger<GetUsersInRegionRequestHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly GetUsersInRegionRequestHandler _handler;

    public GetUsersInRegionRequestHandlerTests()
    {
        _logger = Substitute.For<ILogger<GetUsersInRegionRequestHandler>>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new GetUsersInRegionRequestHandler(_logger, _userRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnUsersSuccessfully()
    {
        // Arrange
        var polygon = new Polygon(new LinearRing(new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(1, 0),
            new Coordinate(0, 0)
        }));
        var request = new GetUsersInRegionRequest(polygon);
        var users = new List<User>
        {
            new User("User1", new Point(0.5, 0.5)),
            new User("User2", new Point(0.6, 0.6))
        };
        _userRepository.GetAllUsersInRegion(Arg.Any<Polygon>())
            .Returns(users);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoUsersFound()
    {
        // Arrange
        var polygon = new Polygon(new LinearRing(new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(1, 0),
            new Coordinate(0, 0)
        }));
        var request = new GetUsersInRegionRequest(polygon);
        _userRepository.GetAllUsersInRegion(Arg.Any<Polygon>())
            .Returns((IEnumerable<User>)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("No users found");
    }

    [Fact]
    public async Task Handle_ShouldReturnNoUsersFoundWhenListIsEmpty()
    {
        // Arrange
        var polygon = new Polygon(new LinearRing(new[]
        {
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(1, 0),
            new Coordinate(0, 0)
        }));
        var request = new GetUsersInRegionRequest(polygon);
        _userRepository.GetAllUsersInRegion(Arg.Any<Polygon>())
            .Returns(new List<User>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("No users found");
    }
}
