using Atlas.Api.Application.Commands;
using Atlas.Api.Application.Dto;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Atlas.UnitTests.Application;

public class CreatePointOfInterestRequestHandlerTests
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository;
    private readonly ILogger<CreatePointOfInterestRequestHandler> _logger;
    private readonly CreatePointOfInterestRequestHandler _handler;

    public CreatePointOfInterestRequestHandlerTests()
    {
        _PointOfInterestRepository = Substitute.For<IPointOfInterestRepository>();
        _logger = Substitute.For<ILogger<CreatePointOfInterestRequestHandler>>();
        _handler = new CreatePointOfInterestRequestHandler(_PointOfInterestRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldCreatePointOfInterestSuccessfully()
    {
        // Arrange
        var request = new CreatePointOfInterestRequest("testPointOfInterest", "testDescription", new CoordinateDto(10.0, 20.0));
        _PointOfInterestRepository.PointOfInterestNameExistAsync(Arg.Any<string>()).Returns(false);
        _PointOfInterestRepository.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("testPointOfInterest");
        await _PointOfInterestRepository.Received(1).AddAsync(Arg.Any<PointOfInterest>());
        await _PointOfInterestRepository.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureWhenPointOfInterestnameExists()
    {
        // Arrange
        var request = new CreatePointOfInterestRequest("testPointOfInterest", "testDescription", new CoordinateDto(10.0, 20.0));
        _PointOfInterestRepository.PointOfInterestNameExistAsync(Arg.Any<string>()).Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "PointOfInterestname already exist");
        await _PointOfInterestRepository.DidNotReceive().AddAsync(Arg.Any<PointOfInterest>());
        await _PointOfInterestRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
