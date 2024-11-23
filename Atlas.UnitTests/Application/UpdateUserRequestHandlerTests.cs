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
using Atlas.Api.Application.Commands.User;

namespace Atlas.UnitTests.Application;

public class UpdateUserRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserRequestHandler> _logger;
    private readonly UpdateUserRequestHandler _handler;

    public UpdateUserRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UpdateUserRequestHandler>>();
        _handler = new UpdateUserRequestHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto(userId,"updateduser", Latitude : 10.0, Longitude : 20.0 );
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _userRepository.GetAsync(userId).Returns(user);
        _userRepository.UsernameExistAsync(userDto.Username).Returns(false);
        _userRepository.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

        var request = new UpdateUserRequest(userId, userDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("updateduser");
        _userRepository.Received(1).Update(user);
        await _userRepository.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto(userId, "updateduser", Latitude: 10.0, Longitude: 20.0);
        _userRepository.GetAsync(userId).Returns((User)null);

        var request = new UpdateUserRequest(userId, userDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User not found");
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _userRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUsernameAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto(userId, "updateduser", Latitude: 10.0, Longitude: 20.0);
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _userRepository.GetAsync(userId).Returns(user);
        _userRepository.UsernameExistAsync(userDto.Username).Returns(true);

        var request = new UpdateUserRequest(userId, userDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Username already exist");
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _userRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
