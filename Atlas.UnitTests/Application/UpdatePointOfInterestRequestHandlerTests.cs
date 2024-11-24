using Atlas.Api.Application.Dto;
using Atlas.Api.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Api.Application.Commands.Poi;
using Atlas.Api.Infrastructure.Contracts;

namespace Atlas.UnitTests.Application;

public class UpdatePointOfInterestRequestHandlerTests
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository;
    private readonly ILogger<UpdatePointOfInterestRequestHandler> _logger;
    private readonly UpdatePointOfInterestRequestHandler _handler;

    public UpdatePointOfInterestRequestHandlerTests()
    {
        _PointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _logger = Substitute.For<ILogger<UpdatePointOfInterestRequestHandler>>();
        _handler = new UpdatePointOfInterestRequestHandler(_PointOfInterestRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePointOfInterestSuccessfully()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        var PointOfInterestDto = new PointOfInterestDto(PointOfInterestId,"updatedPointOfInterest", "testDescription", Latitude : 10.0, Longitude : 20.0 );
        var PointOfInterest = new PointOfInterest("testPointOfInterest", "testDescription", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns(PointOfInterest);
        _PointOfInterestRepository.PointOfInterestNameExistAsync(PointOfInterestDto.Name).Returns(false);
        _PointOfInterestRepository.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

        var request = new UpdatePointOfInterestRequest(PointOfInterestId, PointOfInterestDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("updatedPointOfInterest");
        _PointOfInterestRepository.Received(1).Update(PointOfInterest);
        await _PointOfInterestRepository.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPointOfInterestIsNotFound()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        var PointOfInterestDto = new PointOfInterestDto(PointOfInterestId, "updatedPointOfInterest", "testDescription",Latitude: 10.0, Longitude: 20.0);
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns((PointOfInterest)null);

        var request = new UpdatePointOfInterestRequest(PointOfInterestId, PointOfInterestDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "PointOfInterest not found");
        _PointOfInterestRepository.DidNotReceive().Update(Arg.Any<PointOfInterest>());
        await _PointOfInterestRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPointOfInterestnameAlreadyExists()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        var PointOfInterestDto = new PointOfInterestDto(PointOfInterestId, "updatedPointOfInterest", "testDescription", Latitude: 10.0, Longitude: 20.0);
        var PointOfInterest = new PointOfInterest("testPointOfInterest", "testDescription", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns(PointOfInterest);
        _PointOfInterestRepository.PointOfInterestNameExistAsync(PointOfInterestDto.Name).Returns(true);

        var request = new UpdatePointOfInterestRequest(PointOfInterestId, PointOfInterestDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "PointOfInterestname already exist");
        _PointOfInterestRepository.DidNotReceive().Update(Arg.Any<PointOfInterest>());
        await _PointOfInterestRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
