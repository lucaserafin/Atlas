using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Api.Infrastructure.Contracts;

namespace Atlas.UnitTests.Application;

public class GetNearPointOfInterestRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPointOfInterestRepository _pointOfInterestRepository;
    private readonly ILogger<GetNearPointOfInterestRequestHandler> _logger;
    private readonly GetNearPointOfInterestRequestHandler _handler;

    public GetNearPointOfInterestRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _pointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _logger = Substitute.For<ILogger<GetNearPointOfInterestRequestHandler>>();
        _handler = new GetNearPointOfInterestRequestHandler(_logger, _userRepository, _pointOfInterestRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnPointsOfInterestSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var distance = 100.0;
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        var pointsOfInterest = new List<PointOfInterest>
        {
            new PointOfInterest ( name : "POI1", description: "Description Poi1",location : new NetTopologySuite.Geometries.Point(10.1, 20.1) ),
            new PointOfInterest ( name : "POI2", description: "Description Poi2",location : new NetTopologySuite.Geometries.Point(10.2, 20.2) )
        };
        _userRepository.GetAsync(userId).Returns(user);
        _pointOfInterestRepository.GetNearPointOfInterestAsync(user.Location, distance).Returns(pointsOfInterest);

        var request = new GetNearPointOfInterestRequest(userId, distance);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.First().Name.Should().Be("POI1");
        result.Value.Last().Name.Should().Be("POI2");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var distance = 100.0;
        _userRepository.GetAsync(userId).Returns((User)null);

        var request = new GetNearPointOfInterestRequest(userId, distance);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoPointsOfInterestFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var distance = 100.0;
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _userRepository.GetAsync(userId).Returns(user);
        _pointOfInterestRepository.GetNearPointOfInterestAsync(user.Location, distance).Returns((IEnumerable<PointOfInterest>)null);

        var request = new GetNearPointOfInterestRequest(userId, distance);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No point of interest found near user");
    }
}
