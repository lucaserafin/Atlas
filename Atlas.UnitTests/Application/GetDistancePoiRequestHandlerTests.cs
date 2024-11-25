using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Infrastructure.Contracts;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NetTopologySuite.Geometries;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Atlas.Api.Domain;

namespace Atlas.UnitTests.Application;

public class GetDistancePoiRequestHandlerTests
{
    private readonly ILogger<GetDistancePoiRequestHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPointOfInterestRepository _pointOfInterestRepository;
    private readonly GetDistancePoiRequestHandler _handler;

    public GetDistancePoiRequestHandlerTests()
    {
        _logger = Substitute.For<ILogger<GetDistancePoiRequestHandler>>();
        _userRepository = Substitute.For<IUserRepository>();
        _pointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _handler = new GetDistancePoiRequestHandler(_logger, _userRepository, _pointOfInterestRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnDistanceSuccessfully()
    {
        // Arrange
        var request = new GetDistancePoiRequest(Guid.NewGuid(), Guid.NewGuid());
        var user = new User("User1", new Point(10.0, 20.0));
        var poi = new PointOfInterest("POI1", "test description",new Point(30.0, 40.0));
        _userRepository.GetAsync(request.UserId).Returns(user);
        _pointOfInterestRepository.GetAsync(request.PoiId).Returns(poi);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeApproximately(2928.21, 0.01);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoUserFound()
    {
        // Arrange
        var request = new GetDistancePoiRequest(Guid.NewGuid(), Guid.NewGuid());
        _userRepository.GetAsync(request.UserId).Returns((User)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("No user found");
    }

    [Fact]
    public async Task Handle_ShouldReturnNoPoiFound()
    {
        // Arrange
        var request = new GetDistancePoiRequest(Guid.NewGuid(), Guid.NewGuid());
        var user = new User("User1", new Point(10.0, 20.0));
        _userRepository.GetAsync(request.UserId).Returns(user);
        _pointOfInterestRepository.GetAsync(request.PoiId).Returns((PointOfInterest)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("No point of interest found");
    }

}
