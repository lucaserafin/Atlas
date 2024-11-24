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
using Atlas.Api.Infrastructure.Contracts;

namespace Atlas.UnitTests.Application;

public class GetUserRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserRequestHandler> _logger;
    private readonly GetUserRequestHandler _handler;

    public GetUserRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetUserRequestHandler>>();
        _handler = new GetUserRequestHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserIsFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", new NetTopologySuite.Geometries.Point(10.0, 20.0));
        _userRepository.GetAsync(userId).Returns(user);

        var request = new GetUserRequest(userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("testuser");
        await _userRepository.Received(1).GetAsync(userId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetAsync(userId).Returns((User)null);

        var request = new GetUserRequest(userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User not found");
        await _userRepository.Received(1).GetAsync(userId);
    }
}
