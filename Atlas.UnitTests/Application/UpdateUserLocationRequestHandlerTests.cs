using Atlas.Api.Application.Commands.User;
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
using Atlas.Api.Infrastructure;

namespace Atlas.UnitTests.Application;

public class UpdateUserLocationRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserLocationRequestHandler> _logger;
    private readonly UpdateUserLocationRequestHandler _handler;

    public UpdateUserLocationRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UpdateUserLocationRequestHandler>>();
        _handler = new UpdateUserLocationRequestHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserLocationSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var coordinateDto = new CoordinateDto (Latitude :10.0, Longitude :20.0 );
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _userRepository.GetAsync(userId).Returns(user);

        var request = new UpdateUserLocationRequest(userId, coordinateDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).GetAsync(userId);
        user.Location.Y.Should().Be(10.0);
        user.Location.X.Should().Be(20.0);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var coordinateDto = new CoordinateDto(Latitude: 10.0, Longitude: 20.0);
        _userRepository.GetAsync(userId).Returns((User)null);

        var request = new UpdateUserLocationRequest(userId, coordinateDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User not found");
        await _userRepository.Received(1).GetAsync(userId);
    }
}
