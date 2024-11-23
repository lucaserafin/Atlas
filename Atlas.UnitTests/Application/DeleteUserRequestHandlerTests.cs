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

public class DeleteUserRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserRequestHandler> _logger;
    private readonly DeleteUserRequestHandler _handler;

    public DeleteUserRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<DeleteUserRequestHandler>>();
        _handler = new DeleteUserRequestHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _userRepository.GetAsync(userId).Returns(user);
        _userRepository.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

        var request = new DeleteUserRequest(userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepository.Received(1).Remove(user);
        await _userRepository.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetAsync(userId).Returns((User)null);

        var request = new DeleteUserRequest(userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User not found");
        _userRepository.DidNotReceive().Remove(Arg.Any<User>());
        await _userRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
