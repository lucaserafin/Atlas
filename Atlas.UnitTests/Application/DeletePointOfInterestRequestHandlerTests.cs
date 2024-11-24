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

public class DeletePointOfInterestRequestHandlerTests
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository;
    private readonly ILogger<DeletePointOfInterestRequestHandler> _logger;
    private readonly DeletePointOfInterestRequestHandler _handler;

    public DeletePointOfInterestRequestHandlerTests()
    {
        _PointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _logger = Substitute.For<ILogger<DeletePointOfInterestRequestHandler>>();
        _handler = new DeletePointOfInterestRequestHandler(_PointOfInterestRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldDeletePointOfInterestSuccessfully()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        var PointOfInterest = new PointOfInterest("testPointOfInterest", "testDescription", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns(PointOfInterest);
        _PointOfInterestRepository.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

        var request = new DeletePointOfInterestRequest(PointOfInterestId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _PointOfInterestRepository.Received(1).Remove(PointOfInterest);
        await _PointOfInterestRepository.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPointOfInterestIsNotFound()
    {
        // Arrange
        var PointOfInterestId = Guid.NewGuid();
        _PointOfInterestRepository.GetAsync(PointOfInterestId).Returns((PointOfInterest)null);

        var request = new DeletePointOfInterestRequest(PointOfInterestId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "PointOfInterest not found");
        _PointOfInterestRepository.DidNotReceive().Remove(Arg.Any<PointOfInterest>());
        await _PointOfInterestRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
