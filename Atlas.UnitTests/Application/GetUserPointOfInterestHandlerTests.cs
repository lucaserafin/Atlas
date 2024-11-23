using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Api.Infrastructure;

namespace Atlas.UnitTests.Application;

public class GetPointOfInterestRequestHandlerTests
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository;
    private readonly ILogger<GetPointOfInterestRequestHandler> _logger;
    private readonly GetPointOfInterestRequestHandler _handler;

    public GetPointOfInterestRequestHandlerTests()
    {
        _PointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _logger = Substitute.For<ILogger<GetPointOfInterestRequestHandler>>();
        _handler = new GetPointOfInterestRequestHandler(_PointOfInterestRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnPointOfInterestDto_WhenPointOfInterestIsFound()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        var PointOfInterest = new PointOfInterest("testPointOfInterest", "testDescription", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns(PointOfInterest);

        var request = new GetPointOfInterestRequest(PointOfInterestId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("testPointOfInterest");
        await _PointOfInterestRepository.Received(1).GetAsync(PointOfInterestId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPointOfInterestIsNotFound()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns((PointOfInterest)null);

        var request = new GetPointOfInterestRequest(PointOfInterestId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "PointOfInterest not found");
        await _PointOfInterestRepository.Received(1).GetAsync(PointOfInterestId);
    }
}
