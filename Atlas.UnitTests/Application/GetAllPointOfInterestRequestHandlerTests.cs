using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Api.Infrastructure.Contracts;

namespace Atlas.UnitTests.Application;

public class GetAllPointOfInterestRequestHandlerTests
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository;
    private readonly ILogger<GetAllPointOfInterestRequestHandler> _logger;
    private readonly GetAllPointOfInterestRequestHandler _handler;

    public GetAllPointOfInterestRequestHandlerTests()
    {
        _PointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _logger = Substitute.For<ILogger<GetAllPointOfInterestRequestHandler>>();
        _handler = new GetAllPointOfInterestRequestHandler(_PointOfInterestRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnPointOfInterestDtos_WhenPointOfInterestsAreFound()
    {
        // Arrange
        var PointOfInterests = new List<PointOfInterest>
        {
            new PointOfInterest("testPointOfInterest1",  "testDescription", new NetTopologySuite.Geometries.Point(10.0, 20.0)),
            new PointOfInterest("testPointOfInterest2", "testDescription", new NetTopologySuite.Geometries.Point(30.0, 40.0))
        };
        _PointOfInterestRepository.GetAllAsync().Returns(PointOfInterests);

        var request = new GetAllPointOfInterestRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.First().Name.Should().Be("testPointOfInterest1");
        result.Value.Last().Name.Should().Be("testPointOfInterest2");
        await _PointOfInterestRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoPointOfInterestsAreFound()
    {
        // Arrange
        _PointOfInterestRepository.GetAllAsync().Returns(Enumerable.Empty<PointOfInterest>());

        var request = new GetAllPointOfInterestRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No PointOfInterests on DB");
        await _PointOfInterestRepository.Received(1).GetAllAsync();
    }
}
